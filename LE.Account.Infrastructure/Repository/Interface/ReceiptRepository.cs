using LE.Account.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface ReceiptRepository
    {
        void saveChanges();
        // Real transaction boundary on the shared AppDbContext (see BaseRepositoryImpl).
        IDbContextTransaction beginTransaction();
        void insert(Receipt receipt);
        void update(Receipt receipt);
        void delete(Receipt receipt);
        List<Receipt> getAll();
        Receipt getById(long receipt_id);
        IQueryable<Receipt> getQueryable();
    }
}
