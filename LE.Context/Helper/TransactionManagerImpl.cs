using LE.Context.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace LE.Context.Helper
{
    public class TransactionManagerImpl : TransactionManager
    {
        [ThreadStatic]
        private static int count = 0;

        private AppDbContext _appDbContext;
        public TransactionManagerImpl(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void beginTransaction()
        {
            if (count == 0)
            {
                //if (_appDbContext.Database.CurrentTransaction == null)
                //{
                _appDbContext.Database.BeginTransaction();
                // }
            }
            count++;
        }

        public void commitTransaction()
        {
            count--;
            if (count == 0)
            {
                //if (_appDbContext.Database.CurrentTransaction != null)
                //{
                _appDbContext.SaveChanges();
                _appDbContext.Database.CommitTransaction();
                // }
            }
        }

        public void rollbackTransaction()
        {
            count--;
            if (count == 0)
            {
                //if (_appDbContext.Database.CurrentTransaction != null)
                //{
                    _appDbContext.Database.RollbackTransaction();
               // }
            }
        }
    }
}
