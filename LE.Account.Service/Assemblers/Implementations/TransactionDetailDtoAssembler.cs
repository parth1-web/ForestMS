using LE.Account.Infrastructure.Dto;
using System.Collections.Generic;

namespace LE.Account.Service.Assemblers.Implementations
{
    public interface TransactionDetailDtoAssembler
    {
        List<TransactionDetailDto> getTransactionDetails(TransactionDto transaction_dto);
    }
}
