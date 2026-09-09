using LE.Account.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Context.Repository.Implementations
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
            return (drBal - crBal);
        }

        public decimal getEndBalance(long ledger_id)
        {
            decimal bal = 0;
            var totalPostBefore = AppDbContext.transaction_detail.Where(a => a.ledger_id == ledger_id).OrderBy(a => a.transaction_date).ToList();
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
        public List<TransactionDetail> getAllTransactionDetailOfLedgerLaterThanDate(long ledger_id, DateTime new_date)
        {
            var data = AppDbContext.transaction_detail.Where(a => a.transaction_date.Date > new_date.Date && a.ledger_id == ledger_id).OrderBy(a => a.transaction_date).ToList();
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
