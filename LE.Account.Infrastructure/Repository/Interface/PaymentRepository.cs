using LE.Account.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface PaymentRepository
    {
        void insert(Payment payment);
        void update(Payment payment);
        void delete(Payment payment);
        List<Payment> getAll();
        Payment getById(long payment_id);
        IQueryable<Payment> getQueryable();
    }
}
