using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Services.Interface;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Billing.Service.Services.Implementations
{
    public class ChiranSalesServiceImpl : ChiranSalesService
    {
        private readonly ChiranSalesRepository _chiranSalesRepo;
        private readonly DayCloseRepository _dayCloseRepository;
        private readonly MemberRepository _memberRepo;
        private readonly ChiranSalesDetailRepository _chiranSalesDetailRepo;
        private readonly ChiranSalesAssembler _chiranSalesAssembler;
        private ChiranSalesDetailService _chiranSalesDetailService;
        private LedgerIdProvider _ledgerIdProvider;
        private TransactionService _transactionService;

        public ChiranSalesServiceImpl(ChiranSalesRepository chiranSalesRepo, ChiranSalesDetailRepository chiranSalesDetailRepo, ChiranSalesAssembler chiranSalesAssembler, ChiranSalesDetailService chiranSalesDetailService, LedgerIdProvider ledgerIdProvider, TransactionService transactionService, MemberRepository memberRepo, DayCloseRepository dayCloseRepository)
        {
            _chiranSalesAssembler = chiranSalesAssembler;
            _chiranSalesRepo = chiranSalesRepo;
            _chiranSalesDetailRepo = chiranSalesDetailRepo;
            _chiranSalesDetailService = chiranSalesDetailService;
            _ledgerIdProvider = ledgerIdProvider;
            _memberRepo = memberRepo;
            _transactionService = transactionService;
            _dayCloseRepository = dayCloseRepository;
        }

        public void cancel(long chiran_sales_id, long user_id)
        {
            var chiranSale = _chiranSalesRepo.getById(chiran_sales_id);
            if (chiranSale == null)
            {
                throw new ItemNotFoundException($"Chiran Bill with id {chiran_sales_id} doesnot exist.");
            }
            if (chiranSale.is_cancelled)
            {
                throw new ItemUsedException("This bill is already cancelled.");
            }

            // P1/B4 fix: counter-bill cancels already enforce day-close; chiran cancels
            // now do too, and the whole cancel runs in one real database transaction.
            var isClosed = _dayCloseRepository.getByDate(chiranSale.sales_date.Date);
            if (isClosed != null)
            {
                throw new ItemUsedException("Day is already closed. You cannot cancel this bill.");
            }

            using (var tx = _chiranSalesRepo.beginTransaction())
            {
                chiranSale.cancel();
                chiranSale.cancelled_by = user_id;
                chiranSale.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                _chiranSalesRepo.update(chiranSale);

                createReverseTransaction(chiranSale);
                tx.Commit();
            }
        }

        private void createReverseTransaction(ChiranSales chiranSale)
        {
            long cashLedgerId = getCashLedgerId();
            long chiranSalesLedgerId = getChiranLedgerId();

            var transactionDto = new TransactionDto();

            if (chiranSale.tax_amount > 0)
            {
                long taxLedgerId = getTaxLedgerId();
                transactionDto.addDebitData(new LedgerTransactionDto()
                {
                    amount = chiranSale.tax_amount,
                    ledger_id = taxLedgerId,
                });

            }

            transactionDto.addDebitData(new LedgerTransactionDto()
            {
                amount = chiranSale.amount,
                ledger_id = chiranSalesLedgerId,
            });

            transactionDto.addCreditData(new LedgerTransactionDto()
            {
                amount = chiranSale.amount + chiranSale.tax_amount,
                ledger_id = cashLedgerId
            });
            transactionDto.remarks = "चिरान बिक्री रद्द";
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transactionDto.voucher_type = VoucherType.ChiranSales;
            transactionDto.voucher_no = chiranSale.chiran_sales_id;
            _transactionService.addTransaction(transactionDto);
        }

        public long insert(ChiranSalesDto chiran_sales_dto)
        {
            // P1/B5 fix: the day-close check was commented out, letting sales be entered
            // on already-closed days. Restored, and the insert now runs in one real
            // database transaction instead of the no-op ambient TransactionScope.
            using (var tx = _chiranSalesRepo.beginTransaction())
            {
                var IsClosed = _dayCloseRepository.getByDate(chiran_sales_dto.sales_date.Date);
                if (IsClosed != null)
                {
                    throw new ItemUsedException("Day is already closed. You cannot perform transactions in this date.");
                }

                var chiranSales = new ChiranSales();

                _chiranSalesAssembler.copy(chiranSales, chiran_sales_dto);

                _chiranSalesRepo.insert(chiranSales);

                chiran_sales_dto.chiran_detail_dto.ForEach(a => a.chiran_sales_id = chiranSales.chiran_sales_id);
                _chiranSalesDetailService.insert(chiran_sales_dto.chiran_detail_dto);

                if (chiran_sales_dto.amount > 0)
                {
                    chiran_sales_dto.chiran_sales_id = chiranSales.chiran_sales_id;
                    createTransaction(chiran_sales_dto);

                }

                tx.Commit();
                return chiranSales.chiran_sales_id;
            }
        }

        private long getChiranLedgerId()
        {
            var chiranSalesLedger = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.chiran_sales);

            if (chiranSalesLedger <= 0)
            {
                throw new ItemNotFoundException("Chiran Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            return chiranSalesLedger;
        }

        private long getCashLedgerId()
        {
            long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (cashLedgerId <= 0)
            {
                throw new ItemNotFoundException("Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            return cashLedgerId;
        }

        private void createTransaction(ChiranSalesDto dto)
        {
            long cashLedgerId = getCashLedgerId();
            long chiranSalesLedgerId = getChiranLedgerId();

            var transactionDto = new TransactionDto();

            if (dto.tax_amount > 0)
            {
                long taxLedgerId = getTaxLedgerId();
                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = dto.tax_amount,
                    ledger_id = taxLedgerId,
                });

            }

            transactionDto.addCreditData(new LedgerTransactionDto()
            {
                amount = dto.amount,
                ledger_id = chiranSalesLedgerId,
            });

            transactionDto.addDebitData(new LedgerTransactionDto()
            {
                amount = dto.amount + dto.tax_amount,
                ledger_id = cashLedgerId
            });
            transactionDto.remarks = "चिरान बिक्रीबाट प्राप्त नगद";
            transactionDto.transaction_date = dto.sales_date;
            transactionDto.voucher_no = dto.chiran_sales_id;
            transactionDto.voucher_type = VoucherType.ChiranSales;
            _transactionService.addTransaction(transactionDto);
        }

        private long getTaxLedgerId()
        {
            long taxLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.Tax);
            if (taxLedgerId <= 0)
            {
                throw new ItemNotFoundException("Tax Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            return taxLedgerId;
        }
    }
}
