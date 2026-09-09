using LE.Account.Repository.Interface;
using LE.Entities.Account;
using LE.Context.Data;
using LE.Common.Repository.Implementations;

namespace LE.Account.Context.Repository.Implementations
{
    public class PaymentRepositoryImpl:BaseRepositoryImpl<Payment>, PaymentRepository
    {
        private readonly AppDbContext AppDbContext;
        public PaymentRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }
    }
}
