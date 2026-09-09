using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class WoodBillDetailRepositoryImpl : BaseRepositoryImpl<WoodBillDetail>, WoodBillDetailRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodBillDetailRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
