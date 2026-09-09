using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Assemblers.Interface
{
    public interface LedgerAssembler
    {
        void copy(Ledger ledger, LedgerDto ledger_dto);
    }
}
