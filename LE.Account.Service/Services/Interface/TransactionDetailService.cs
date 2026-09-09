using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using System;
using System.Threading.Tasks;

namespace LE.Account.Service.Services.Interface
{
    public interface TransactionDetailService
    {
        void addTransactionDetail(TransactionDetail transaction_detail);
        void addTransactionDetail(TransactionDto transaction_dto, long transaction_id);
        decimal getOldBalance(long ledger_id, DateTime last_date);
        decimal getEndBalance(long ledger_id);

        void updateBalanceAmount(long ledger_id);
        Task<FinancialYear> GetRunningFinancialYear();
    }
}
