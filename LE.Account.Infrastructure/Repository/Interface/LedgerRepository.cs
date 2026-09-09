using LE.Account.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface LedgerRepository
    {
        void insert(Ledger ledger);
        void update(Ledger ledger);
        void delete(Ledger ledger);
        List<Ledger> getAll();
        Ledger getById(long ledger_id);
        Ledger getByName(string name);
        IQueryable<Ledger> getQueryable();
        List<Ledger> getLedgersByLedgerGroup(long ledger_group_id);
        List<Ledger> getLedgersUnderAssetsGroup();
        List<Ledger> getLedgersUnderIncomeGroup();
        List<Ledger> getLedgersUnderExpensesGroup();
        List<Ledger> getLedgersUnderLiabilitiesGroup();
    }
}
