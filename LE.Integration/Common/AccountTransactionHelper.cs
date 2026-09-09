using LE.Account.Infrastructure.Dto;

namespace LE.Integration.Common
{
    public interface AccountTransactionHelper
    {
        void makeTransaction(TransactionDto dto);
    }
}
