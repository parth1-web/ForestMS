using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Assemblers.Interface
{
    public interface LedgerBalanceAssembler
    {
        void copy(LedgerBalance ledgerBalance, LedgerBalanceDto ledgerBalance_dto);
    }
}
