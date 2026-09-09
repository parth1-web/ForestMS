using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Services.Interface;
using LE.Billing.Common.Enums;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Integration.Common;
using LE.Inventory.Infrastructure.Repository.Interface;
using System;
using System.Linq;
using System.Transactions;

namespace LE.Integration.Service.Implementations
{
    public class BillingWoodBillServiceImpl
    {
        //billing
        private readonly WoodBillRepository _salesRepo;
        private readonly WoodBillAssembler _salesMaker;
        private readonly WoodBillDetailService _salesDetailService;
        private readonly MemberRepository _memberRepo;

        //inventory
        private readonly WoodDetailsRepository _woodDetailsRepo;

        //account
        private readonly LedgerIdProvider _ledgerIdProvider;
        private readonly AccountTransactionHelper _accountTransactionHelper;

        public BillingWoodBillServiceImpl(WoodBillRepository woodBillRepo, WoodBillAssembler salesMaker, WoodBillDetailService woodBillDetailService, MemberRepository memberRepo, LedgerIdProvider ledgerIdProvider, AccountTransactionHelper accountTransactionHelper, WoodDetailsRepository woodDetailsRepo)
        {
            _salesRepo = woodBillRepo;
            _salesMaker = salesMaker;
            _salesDetailService = woodBillDetailService;
            _ledgerIdProvider = ledgerIdProvider;
            _accountTransactionHelper = accountTransactionHelper;
            _memberRepo = memberRepo;
            _woodDetailsRepo = woodDetailsRepo;
        }

        public long insert(WoodBillDto wood_bill_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {

                    long salesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.sales);
                    long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);

                    //updating woodDetails
                    foreach (var detail in wood_bill_dto.wood_detail_dto)
                    {
                        var woodDetail = _woodDetailsRepo.getById(detail.wood_details_id);
                        woodDetail.is_sold = true;
                        _woodDetailsRepo.update(woodDetail);

                    }

                    if (wood_bill_dto.sales_type == SalesType.Member)
                    {
                        foreach (var member in wood_bill_dto.wood_bill_member_dto)
                        {
                            var memberDetail = _memberRepo.getById(member.member_id);
                            //long memberLedgerId = memberDetail.ledger_id;

                            var transactionDto = new TransactionDto();
                            var transactionDto1 = new TransactionDto();
                            decimal totalAmt = 0;
                            foreach (var x in wood_bill_dto.wood_bill_member_transaction_dto.Where(x => x.member_id == member.member_id))
                            {
                                totalAmt += x.amount;
                            }
                            transactionDto.addCreditData(new LedgerTransactionDto()
                            {
                                amount = totalAmt,
                                ledger_id = salesLedgerId,
                                // ref_ledger_id = memberLedgerId
                            });

                            transactionDto.addDebitData(new LedgerTransactionDto()
                            {
                                amount = totalAmt,
                                //   ref_ledger_id = salesLedgerId,
                                //ledger_id = memberLedgerId
                            });

                            transactionDto1.addCreditData(new LedgerTransactionDto()
                            {
                                amount = totalAmt,
                                //ledger_id = memberLedgerId,
                                //   ref_ledger_id = cashLedgerId
                            });

                            transactionDto1.addDebitData(new LedgerTransactionDto()
                            {
                                amount = totalAmt,
                                // ref_ledger_id = memberLedgerId,
                                ledger_id = cashLedgerId
                            });
                            _accountTransactionHelper.makeTransaction(transactionDto);
                            _accountTransactionHelper.makeTransaction(transactionDto1);

                        }
                    }

                    else if (wood_bill_dto.sales_type == SalesType.Others)
                    {
                        decimal amt = 0;
                        var transactionDto = new TransactionDto();
                        foreach (var x in wood_bill_dto.wood_detail_dto)
                        {
                            amt += x.amount;

                        }
                        transactionDto.addCreditData(new LedgerTransactionDto()
                        {
                            amount = amt,
                            ledger_id = salesLedgerId,
                            // ref_ledger_id = cashLedgerId
                        });

                        transactionDto.addDebitData(new LedgerTransactionDto()
                        {
                            amount = amt,
                            //  ref_ledger_id = salesLedgerId,
                            ledger_id = cashLedgerId
                        });
                        _accountTransactionHelper.makeTransaction(transactionDto);
                    }
                    tx.Complete();
                    return wood_bill_dto.wood_bill_id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void cancel(long wood_bill_id)
        {
            throw new NotImplementedException();
        }
    }
}
