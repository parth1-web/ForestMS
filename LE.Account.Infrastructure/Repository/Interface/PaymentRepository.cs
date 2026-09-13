using LE.Account.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface PaymentRepository
    {
        // Real transaction boundary on the shared AppDbContext (see BaseRepositoryImpl).
        IDbContextTransaction beginTransaction();
        void insert(Payment payment);
        void update(Payment payment);
        void delete(Payment payment);
        List<Payment> getAll();
        Payment getById(long payment_id);
        IQueryable<Payment> getQueryable();
    }
}
