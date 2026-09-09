using LE.Account.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface LedgerBalanceRepository
    {
        void insert(LedgerBalance ledgerBalance);
        void update(LedgerBalance ledgerBalance);
        void delete(LedgerBalance ledgerBalance);
        List<LedgerBalance> getAll();
        LedgerBalance getById(long ledgerBalance_id);
        IQueryable<LedgerBalance> getQueryable();
    }
}
