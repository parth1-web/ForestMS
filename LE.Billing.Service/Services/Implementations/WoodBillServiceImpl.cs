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
using LE.Inventory.Common.Enums;
using LE.Inventory.Infrastructure.Repository.Interface;
using System;
using System.Linq;
using System.Transactions;

namespace LE.Billing.Service.Services.Implementations
{
    public class WoodBillServiceImpl : WoodBillService
    {
        private readonly WoodBillRepository _woodBillRepository;
        private readonly DayCloseRepository _dayCloseRepository;
        private readonly WoodBillMemberTransactionRepository _woodBillMemberTransactionRepo;
        private readonly MemberRepository _memberRepo;
        private readonly WoodBillDetailRepository _woodBillDetailRepository;
        private readonly WoodBillAssembler _woodBillAssembler;
        private WoodBillDetailService _woodBillDetailService;
        private WoodBillMemberService _woodBillMemberService;
        private WoodBillMemberTransactionService _woodBillMemberTransactionService;
        private WoodDetailsRepository _woodDetailsRepository;
        private LedgerIdProvider _ledgerIdProvider;
        private TransactionService _transactionService;

        public WoodBillServiceImpl(WoodBillMemberTransactionService woodBillMemberTransactionService, WoodBillMemberService woodBillMemberService, WoodBillRepository woodBillRepo, WoodBillDetailRepository woodBillDetailRepository, WoodBillAssembler woodBillAssembler, WoodBillDetailService woodBillDetailService, WoodDetailsRepository woodDetailsRepository, LedgerIdProvider ledgerIdProvider, TransactionService transactionService, WoodBillMemberTransactionRepository woodBillMemberTransactionRepo, MemberRepository memberRepo, DayCloseRepository dayCloseRepository, WoodBillMemberTransactionRepository woodBillMemberTransactionRepository)
        {
            _woodBillAssembler = woodBillAssembler;
            _woodBillRepository = woodBillRepo;
            _woodBillDetailService = woodBillDetailService;
            _woodDetailsRepository = woodDetailsRepository;
            _ledgerIdProvider = ledgerIdProvider;
            _memberRepo = memberRepo;
            _transactionService = transactionService;
            _woodBillMemberService = woodBillMemberService;
            _woodBillMemberTransactionService = woodBillMemberTransactionService;
            _dayCloseRepository = dayCloseRepository;
            _woodBillMemberTransactionRepo = woodBillMemberTransactionRepository;
            _woodBillDetailRepository = woodBillDetailRepository;
        }

        public void cancel(long wood_bill_id, long user_id)
        {
            try
            {
                var billDetail = _woodBillRepository.getById(wood_bill_id);
                if (billDetail == null)
                {
                    throw new ItemNotFoundException($"Wood Bill with id {wood_bill_id} doesnot exist.");
                }
                if (billDetail.is_cancelled)
                {
                    throw new ItemUsedException("This Bill is Already Cancelled.");
                }
                udateBill(user_id, billDetail);
                udateWoodDetails(billDetail);
                updateMemberTransactions(billDetail);
                makeReverseEntryToAccount(billDetail);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void makeReverseEntryToAccount(WoodBill billDetail)
        {
            long cashLedgerId = getCashLedgerId();
            long taxLedgerId = getTaxLedgerId();
            long ballaballiSalesLedger = getBallaballiSalesLedgerId();
            long lakadiSalesLedger = getLakadiSalesLedgerId();

            if (ballaballiSalesLedger <= 0 && lakadiSalesLedger <= 0)
            {
                throw new ItemNotFoundException("Ballaballi and Lakadi Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }


            long salesLedgerId = 0;
            long woodDetailId = _woodBillDetailRepository.getQueryable().Where(a => a.wood_bill_id == billDetail.wood_bill_id).Select(a => a.wood_details_id).FirstOrDefault();
            long woodDetailType = _woodDetailsRepository.getQueryable().Where(a => a.wood_details_id == woodDetailId).Select(a => a.stock_type_id).FirstOrDefault();
            if (woodDetailType == Convert.ToInt32(StockTypes.BallaBalli))
            {
                salesLedgerId = ballaballiSalesLedger;
            }
            if (woodDetailType == Convert.ToInt32(StockTypes.Lakadi))
            {
                salesLedgerId = lakadiSalesLedger;
            }
            if (billDetail.amount > 0)
            {
                createReverseTransactoin(billDetail.amount, salesLedgerId, cashLedgerId, billDetail.tax_amount, taxLedgerId, billDetail.wood_bill_id);

            }
        }

        private long getLakadiSalesLedgerId()
        {
            var lakadiSalesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.lakadi_sales);
            if (lakadiSalesLedgerId <= 0)
            {
                throw new ItemNotFoundException("Lakadi Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            return lakadiSalesLedgerId;
        }

        private long getBallaballiSalesLedgerId()
        {
            var ballaballiSalesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.ballaballi_sales);
            if (ballaballiSalesLedgerId <= 0)
            {
                throw new ItemNotFoundException("Ballaballi Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            return ballaballiSalesLedgerId;
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

        private long getCashLedgerId()
        {
            long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (cashLedgerId <= 0)
            {
                throw new ItemNotFoundException("Wood Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            return cashLedgerId;
        }

        private void udateBill(long user_id, WoodBill billDetail)
        {
            billDetail.cancel();
            billDetail.cancelled_by = user_id;
            billDetail.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            _woodBillRepository.update(billDetail);
        }

        private void updateMemberTransactions(WoodBill billDetail)
        {
            foreach (var memberTransaction in billDetail.WoodBillMemberTransactions)
            {
                memberTransaction.is_cancelled = true;
                _woodBillMemberTransactionRepo.update(memberTransaction);
            }
        }

        private void udateWoodDetails(WoodBill billDetail)
        {
            foreach (var detail in billDetail.wood_bill_detail)
            {
                var wood = _woodDetailsRepository.getById(detail.wood_details_id);
                wood.is_sold = false;
                _woodDetailsRepository.update(wood);
            }
        }

        private void createReverseTransactoin(decimal amount, long salesLedgerId, long cashLedgerId, decimal tax_amount, long taxLedgerId, long wood_bill_id)
        {
            var transactionDto = new TransactionDto();

            transactionDto.addDebitData(new LedgerTransactionDto()
            {
                amount = amount,
                ledger_id = salesLedgerId,
            });
            if (tax_amount > 0)
            {
                transactionDto.addDebitData(new LedgerTransactionDto()
                {
                    amount = tax_amount,
                    ledger_id = taxLedgerId,
                });
            }

            transactionDto.addCreditData(new LedgerTransactionDto()
            {
                amount = amount + tax_amount,
                ledger_id = cashLedgerId
            });
            transactionDto.remarks = "काठ बिक्री रद्द";
            transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            transactionDto.voucher_no = wood_bill_id;
            transactionDto.voucher_type = VoucherType.WoodSales;
            _transactionService.addTransaction(transactionDto);
        }

        public long insert(WoodBillDto wood_bill_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var IsClosed = _dayCloseRepository.getByDate(wood_bill_dto.bill_date.Date);
                    if (IsClosed != null)
                    {
                        throw new ItemUsedException("Day is already closed. You cannot perform transactions in this date.");
                    }

                    var woodBill = new WoodBill();

                    _woodBillAssembler.copy(woodBill, wood_bill_dto);

                    _woodBillRepository.insert(woodBill);

                    wood_bill_dto.wood_detail_dto.ForEach(a => a.wood_bill_id = woodBill.wood_bill_id);
                    _woodBillDetailService.insert(wood_bill_dto.wood_detail_dto);

                    if (wood_bill_dto.wood_bill_member_dto.Count > 0)
                    {

                        wood_bill_dto.wood_bill_member_dto.ForEach(a => a.wood_bill_id = woodBill.wood_bill_id);
                        _woodBillMemberService.insert(wood_bill_dto.wood_bill_member_dto);

                    }

                    if (wood_bill_dto.wood_bill_member_transaction_dto.Count > 0)
                    {
                        wood_bill_dto.wood_bill_member_transaction_dto.ForEach(a => a.wood_bill_id = woodBill.wood_bill_id);
                        _woodBillMemberTransactionService.insert(wood_bill_dto.wood_bill_member_transaction_dto);

                    }



                    //updating woodDetails
                    foreach (var detail in wood_bill_dto.wood_detail_dto)
                    {
                        var woodDetail = _woodDetailsRepository.getById(detail.wood_details_id);
                        woodDetail.is_sold = true;
                        woodDetail.sales_id = woodBill.wood_bill_id;
                        _woodDetailsRepository.update(woodDetail);

                    }

                    var grouped = wood_bill_dto.wood_detail_dto.GroupBy(a => a.stock_type_id).Select(g => new { amount = g.Sum(x => x.amount), salesTypeId = g.Max(a => a.stock_type_id) });

                    long cashLedgerId = getCashLedgerId();

                    long taxLedgerId = getTaxLedgerId();

                    var ballaballiSalesLedger = getBallaballiSalesLedgerId();

                    var lakadiSalesLedger = getLakadiSalesLedgerId();

                    foreach (var groupData in grouped)
                    {
                        decimal taxAmount = wood_bill_dto.tax_amount;
                        decimal salesAmount = groupData.amount;
                        long salesLedgerId = 0;
                        DateTime transactionDate = wood_bill_dto.bill_date;
                        if (Convert.ToInt32(groupData.salesTypeId) == Convert.ToInt32(StockTypes.BallaBalli))
                        {
                            salesLedgerId = ballaballiSalesLedger;
                        }
                        if (Convert.ToInt32(groupData.salesTypeId) == Convert.ToInt32(StockTypes.Lakadi))
                        {
                            salesLedgerId = lakadiSalesLedger;
                        }
                        if (salesAmount > 0)
                        {
                            createTransactoin(salesAmount, salesLedgerId, cashLedgerId, taxAmount, taxLedgerId, transactionDate, woodBill.wood_bill_id);

                        }

                    }

                    tx.Complete();
                    return woodBill.wood_bill_id;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void createTransactoin(decimal amt, long ledgerId, long cashLedgerId, decimal taxAmount, long taxLedgerId, DateTime transactionDate, long woodBillId)
        {
            var transactionDto = new TransactionDto();
            transactionDto.addCreditData(new LedgerTransactionDto()
            {
                amount = amt,
                ledger_id = ledgerId,
            });
            if (taxAmount > 0)
            {
                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = taxAmount,
                    ledger_id = taxLedgerId,
                });
            }

            transactionDto.addDebitData(new LedgerTransactionDto()
            {
                amount = amt + taxAmount,
                ledger_id = cashLedgerId
            });
            transactionDto.remarks = "काठ बिक्रीबाट प्राप्त नगद";
            transactionDto.transaction_date = transactionDate;
            transactionDto.voucher_type = VoucherType.WoodSales;
            transactionDto.voucher_no = woodBillId;
            _transactionService.addTransaction(transactionDto);
        }


    }
}
