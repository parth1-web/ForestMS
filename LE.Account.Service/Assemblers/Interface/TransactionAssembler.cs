using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Assemblers.Interface
{
    public interface TransactionAssembler
    {
        void copy(Transaction transaction, TransactionDto transaction_dto);
    }
}