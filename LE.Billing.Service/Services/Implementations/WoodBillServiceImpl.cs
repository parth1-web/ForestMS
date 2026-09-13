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
            var billDetail = _woodBillRepository.getById(wood_bill_id);
            if (billDetail == null)
            {
                throw new ItemNotFoundException($"Wood Bill with id {wood_bill_id} doesnot exist.");
            }
            if (billDetail.is_cancelled)
            {
                throw new ItemUsedException("This Bill is Already Cancelled.");
            }

            // P1/B4 fix: cancelling must not be possible once the bill date has been day-closed
            // (counter sales already enforced this; wood bills did not).
            var isClosed = _dayCloseRepository.getByDate(billDetail.bill_date.Date);
            if (isClosed != null)
            {
                throw new ItemUsedException("Day is already closed. You cannot cancel this bill.");
            }

            // P1/B4 fix: cancel ran without any real transaction, leaving half-cancelled bills
            // on failure. The EF ambient TransactionScope was a no-op, so all steps now run
            // inside one explicit database transaction on the shared DbContext.
            using (var tx = _woodBillRepository.beginTransaction())
            {
                udateBill(user_id, billDetail);
                udateWoodDetails(billDetail);
                updateMemberTransactions(billDetail);
                makeReverseEntryToAccount(billDetail);
                tx.Commit();
            }
        }

        private void makeReverseEntryToAccount(WoodBill billDetail)
        {
            long cashLedgerId = getCashLedgerId();
            long taxLedgerId = getTaxLedgerId();

            // P1/B4 fix: reverse entries used the first detail's stock type for the whole bill.
            // A mixed bill (ballaballi + lakadi) now reverses each group to its own ledger.
            var grouped = billDetail.wood_bill_detail
                .GroupBy(d => d.woodDetails.stock_type_id)
                .Select(g => new { stockTypeId = g.Key, amount = g.Sum(x => x.amount) });

            foreach (var groupData in grouped)
            {
                long salesLedgerId = getSalesLedgerIdFor(groupData.stockTypeId, requireBoth: false);
                if (billDetail.amount > 0)
                {
                    // P1/B3 fix: tax was posted once per stock-type group with the full bill tax,
                    // overstating cash and tax by (groups - 1). The reverse entry allocates the
                    // bill tax across groups proportionally so the sum equals the posted tax once.
                    decimal groupShare = groupData.amount / billDetail.amount;
                    decimal groupTax = decimal.Round(billDetail.tax_amount * groupShare, 2);
                    createReverseTransactoin(groupData.amount, salesLedgerId, cashLedgerId, groupTax, taxLedgerId, billDetail.wood_bill_id);
                }
            }
        }

        private long getSalesLedgerIdFor(long stockTypeId, bool requireBoth)
        {
            long ballaballiSalesLedger = getBallaballiSalesLedgerId();
            long lakadiSalesLedger = getLakadiSalesLedgerId();

            if (ballaballiSalesLedger <= 0 && lakadiSalesLedger <= 0)
            {
                throw new ItemNotFoundException("Ballaballi and Lakadi Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            if (stockTypeId == Convert.ToInt32(StockTypes.BallaBalli))
            {
                return ballaballiSalesLedger;
            }
            if (stockTypeId == Convert.ToInt32(StockTypes.Lakadi))
            {
                return lakadiSalesLedger;
            }
            return 0;
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
            var woodBill = new WoodBill();

            // P1/B4-related fix: the EF ambient TransactionScope here was a no-op (EF Core 3.1
            // does not enlist in ambient System.Transactions — the warning is suppressed in
            // Startup). All writes now happen inside one explicit database transaction on the
            // shared scoped DbContext, so a failure anywhere rolls the whole bill back.
            using (var tx = _woodBillRepository.beginTransaction())
            {
                var IsClosed = _dayCloseRepository.getByDate(wood_bill_dto.bill_date.Date);
                if (IsClosed != null)
                {
                    throw new ItemUsedException("Day is already closed. You cannot perform transactions in this date.");
                }

                _woodBillAssembler.copy(woodBill, wood_bill_dto);

                _woodBillRepository.insert(woodBill);

                // P1/B1 fix: double-selling guard. Each wood log must transition from
                // unsold to sold atomically; the conditional UPDATE claims only rows
                // still marked unsold, so if another bill just sold the same log it
                // affects fewer rows than expected and we reject the whole bill.
                markWoodDetailsAsSold(wood_bill_dto, woodBill);

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

                postAccountTransaction(wood_bill_dto, woodBill);

                tx.Commit();
            }

            return woodBill.wood_bill_id;
        }

        private void markWoodDetailsAsSold(WoodBillDto wood_bill_dto, WoodBill woodBill)
        {
            // Atomic claim: rows already sold by a concurrent bill are not updated,
            // so a row missing from the count means it was double-sold — reject.
            var ids = wood_bill_dto.wood_detail_dto.Select(a => a.wood_details_id).ToList();
            int claimed = _woodDetailsRepository.markSoldIfNotSold(ids, woodBill.wood_bill_id);
            if (claimed != ids.Distinct().Count())
            {
                var claimedIds = _woodDetailsRepository.getQueryable()
                    .Where(a => ids.Contains(a.wood_details_id) && a.is_sold && a.sales_id != woodBill.wood_bill_id)
                    .Select(a => a.goliya_number)
                    .ToList();
                var goliyaList = string.Join(", ", claimedIds);
                throw new ItemUsedException($"Goliya {goliyaList} is already sold.");
            }
        }

        private void postAccountTransaction(WoodBillDto wood_bill_dto, WoodBill woodBill)
        {
            var grouped = wood_bill_dto.wood_detail_dto.GroupBy(a => a.stock_type_id).Select(g => new { amount = g.Sum(x => x.amount), salesTypeId = g.Key });

            // P1/B15 fix: the bill total was trusted from the client and never
            // reconciled against the detail sums. The ledger now posts the
            // server-computed sum of details, and a mismatch beyond ±0.02 (float
            // rounding from the browser) rejects the bill instead of posting a
            // wrong amount to the ledgers.
            decimal detailSum = grouped.Sum(g => g.amount);
            if (Math.Abs(detailSum - wood_bill_dto.amount) > 0.02m)
            {
                throw new InvalidValueException($"Bill total ({wood_bill_dto.amount}) does not match the sum of its details ({detailSum}).");
            }
            if (wood_bill_dto.tax_amount < 0)
            {
                throw new InvalidValueException("Tax amount cannot be negative.");
            }

            long cashLedgerId = getCashLedgerId();

            long taxLedgerId = getTaxLedgerId();

            var ballaballiSalesLedger = getBallaballiSalesLedgerId();

            var lakadiSalesLedger = getLakadiSalesLedgerId();

            decimal totalSalesAmount = grouped.Sum(g => g.amount);
            if (totalSalesAmount <= 0)
            {
                return;
            }

            foreach (var groupData in grouped)
            {
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
                    // P1/B3 fix: the full bill tax was credited to the tax ledger once per
                    // stock-type group; a mixed bill overstated cash/tax N-1 times. Tax is
                    // now allocated proportionally to each group so Σgroup tax = bill tax.
                    decimal taxAmount = totalSalesAmount > 0
                        ? decimal.Round(wood_bill_dto.tax_amount * (salesAmount / totalSalesAmount), 2)
                        : 0;
                    createTransactoin(salesAmount, salesLedgerId, cashLedgerId, taxAmount, taxLedgerId, transactionDate, woodBill.wood_bill_id);
                }
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
