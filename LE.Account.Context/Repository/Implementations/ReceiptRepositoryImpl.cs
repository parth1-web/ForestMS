using LE.Account.Repository.Interface;
using LE.Entities.Account;
using LE.Context.Data;
using LE.Common.Repository.Implementations;

namespace LE.Account.Context.Repository.Implementations
{
    public class ReceiptRepositoryImpl:BaseRepositoryImpl<Receipt> , ReceiptRepository
    {
        private readonly AppDbContext AppDbContext;
        public ReceiptRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }
    }

}
