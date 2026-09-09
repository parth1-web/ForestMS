using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Implementations
{
    public class TransactionDetailRepositoryImpl : BaseRepositoryImpl<TransactionDetail>, TransactionDetailRepository
    {
        private AppDbContext AppDbContext;
        public TransactionDetailRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }

        public decimal getOldBalance(long ledger_id, DateTime last_date)
        {
            decimal bal = 0;
            var totalPostBefore = AppDbContext.transaction_detail.Where(a => a.ledger_id == ledger_id && a.transaction_date.Date <= last_date.Date).OrderBy(a => a.transaction_date).ToList();
            if (totalPostBefore.Count != 0)
            {
                bal = totalPostBefore[totalPostBefore.Count - 1].balance;
            }
            return bal;
        }

        public decimal getLedgerBalanceAmountBetweenDates(long ledger_id, DateTime start_date, DateTime last_date)
        {
            decimal drBal = 0;
            decimal crBal = 0;
            var totalPostBefore = AppDbContext.transaction_detail.Where(a => a.ledger_id == ledger_id && a.transaction_date.Date >= start_date.Date && a.transaction_date.Date <= last_date.Date).OrderBy(a => a.transaction_date).ToList();
            foreach (var result in totalPostBefore)
            {
                drBal += result.dr_amount;
                crBal += result.cr_amount;
            }
            return drBal - crBal;
        }

        public decimal getEndBalance(long ledger_id)
        {
            decimal bal = 0;
            var totalPostBefore = AppDbContext.transaction_detail.Where(a => a.ledger_id == ledger_id).OrderBy(a => a.transaction_detail_id).ToList();
            if (totalPostBefore.Count != 0)
            {
                bal = totalPostBefore[totalPostBefore.Count - 1].balance;
            }
            return bal;
        }
        public bool isBackDateEntryMade(long ledger_id, DateTime new_date)
        {
            int count = AppDbContext.transaction_detail.Where(a => a.transaction_date > new_date && a.ledger_id == ledger_id).Count();
            return count > 0 ? true : false;
        }
        public List<TransactionDetail> getTransactionDetailsWithInDate(DateTime date)
        {
            return AppDbContext.transaction_detail.Where(a => a.transaction_date.Date == date.Date).ToList();
        }
        public List<TransactionDetail> getAllTransactionDetailOfLedgerLaterThanDate(long ledger_id, DateTime new_date)
        {
            var data = AppDbContext.transaction_detail.Where(a => a.transaction_date > new_date && a.ledger_id == ledger_id).OrderBy(a => a.transaction_detail_id).ToList();
            return data;
        }
        public List<TransactionDetail> getAllTransactionDetailOfLedgerGroupOnDate(long ledgerGroup_id, DateTime new_date)
        {

            var data = new List<TransactionDetail>();
            if (ledgerGroup_id == 0)
            {
                data = AppDbContext.transaction_detail.Where(a => a.transaction_date.Date == new_date.Date).ToList();
                data = data.Where(a => a.ledger.ledger_group_id == 19 || a.ledger.ledger_group_id == 16).ToList();
            }
            else
            {
                data = AppDbContext.transaction_detail.Where(a => a.transaction_date.Date == new_date.Date && a.ledger.ledger_group_id == ledgerGroup_id).OrderBy(a => a.transaction_detail_id).ToList();
            }
            return data;
        }

        public void updateLedgerBalances(List<TransactionDetail> transaction_details)
        {
            foreach (var result in transaction_details)
            {
                this.update(result);
            }
        }
    }
}
