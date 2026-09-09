using LE.Account.Repository.Interface;
using LE.Entities.Account;
using LE.Context.Data;
using LE.Common.Repository.Implementations;

namespace LE.Account.Context.Repository.Implementations
{
    public class TransactionRepositoryImpl:BaseRepositoryImpl<Transaction>,TransactionRepository
    {
        private readonly AppDbContext _AppDbContext;
        public TransactionRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

    }
}
