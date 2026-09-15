using LE.Account.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface LedgerBalanceRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(LedgerBalance ledgerBalance);
        void update(LedgerBalance ledgerBalance);
        void delete(LedgerBalance ledgerBalance);
        List<LedgerBalance> getAll();
        LedgerBalance getById(long ledgerBalance_id);
        IQueryable<LedgerBalance> getQueryable();
    }
}
