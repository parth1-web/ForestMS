using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;

namespace LE.Account.Infrastructure.Repository.Implementations
{
    public class TransactionRepositoryImpl : BaseRepositoryImpl<Transaction>, TransactionRepository
    {
        private readonly AppDbContext _AppDbContext;
        public TransactionRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }
    }
}
