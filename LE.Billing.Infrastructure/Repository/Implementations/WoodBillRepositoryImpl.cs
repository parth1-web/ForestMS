using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class WoodBillRepositoryImpl : BaseRepositoryImpl<WoodBill>, WoodBillRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodBillRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
