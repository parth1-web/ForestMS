using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using System.Threading.Tasks;

namespace LE.Account.Service.Services.Interface
{
    public interface LedgerService
    {
        Ledger save(LedgerDto ledger_dto);
        void update(LedgerDto ledger_dto);
        void delete(long ledger_id);
    }
}
