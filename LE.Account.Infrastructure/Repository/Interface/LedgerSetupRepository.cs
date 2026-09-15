using LE.Account.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface LedgerSetupRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(LedgerSetup ledgerSetup);
        void update(LedgerSetup ledgerSetup);
        void delete(LedgerSetup ledgerSetup);
        List<LedgerSetup> getAll();
        LedgerSetup getById(long ledger_setup_id);
        LedgerSetup getByKey(string key);
        IQueryable<LedgerSetup> getQueryable();
    }
}
