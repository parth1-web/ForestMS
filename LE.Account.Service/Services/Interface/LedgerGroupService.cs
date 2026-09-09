using LE.Account.Entities;

namespace LE.Account.Service.Services.Interface
{
    public interface LedgerGroupService
    {
        void save(LedgerGroup ledger_group);
        void update(LedgerGroup ledger_group);
        void delete(long ledger_group_id);

    }
}
