using LE.Account.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface TransactionDetailRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(TransactionDetail transaction_detail);
        void update(TransactionDetail transaction_detail);
        void delete(TransactionDetail transaction_detail);
        List<TransactionDetail> getAll();
        decimal getOldBalance(long ledger, DateTime last_date);
        decimal getEndBalance(long ledger_id);
        decimal getLedgerBalanceAmountBetweenDates(long ledger_id, DateTime start_date, DateTime last_date);
        TransactionDetail getById(long transaction_detail_id);
        bool isBackDateEntryMade(long ledger_id, DateTime new_date);
        List<TransactionDetail> getAllTransactionDetailOfLedgerLaterThanDate(long ledger_id, DateTime new_date);
        List<TransactionDetail> getAllTransactionDetailOfLedgerGroupOnDate(long ledgerGroup_id, DateTime new_date);
        void updateLedgerBalances(List<TransactionDetail> listOfUpdatedData);
        IQueryable<TransactionDetail> getQueryable();
        List<TransactionDetail> getTransactionDetailsWithInDate(DateTime date);
    }
}
