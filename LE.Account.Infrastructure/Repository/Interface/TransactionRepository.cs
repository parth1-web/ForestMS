using LE.Account.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface TransactionRepository
    {
        void insert(Transaction transaction);
        void update(Transaction transaction);
        void delete(Transaction transaction);
        List<Transaction> getAll();
        Transaction getById(long transaction_id);
        IQueryable<Transaction> getQueryable();
        IDbContextTransaction beginTransaction();
    }
}
