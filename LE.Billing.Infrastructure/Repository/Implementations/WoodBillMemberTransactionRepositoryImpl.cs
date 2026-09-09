using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class WoodBillMemberTransactionRepositoryImpl : BaseRepositoryImpl<WoodBillMemberTransaction>, WoodBillMemberTransactionRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodBillMemberTransactionRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
