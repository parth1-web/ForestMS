using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;

namespace LE.Account.Infrastructure.Repository.Implementations
{
    public class PaymentRepositoryImpl : BaseRepositoryImpl<Payment>, PaymentRepository
    {
        private readonly AppDbContext AppDbContext;
        public PaymentRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }
    }
}
