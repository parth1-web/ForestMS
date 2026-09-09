using LE.Account.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface ReceiptRepository
    {
        void insert(Receipt receipt);
        void update(Receipt receipt);
        void delete(Receipt receipt);
        List<Receipt> getAll();
        Receipt getById(long receipt_id);
        IQueryable<Receipt> getQueryable();
    }
}
