using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Services.Interface
{
    public interface TransactionService
    {
        void addTransaction(TransactionDto transactionDto);
    }
}
