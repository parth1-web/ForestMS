using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class ChiranSalesRepositoryImpl : BaseRepositoryImpl<ChiranSales>, ChiranSalesRepository
    {
        private readonly AppDbContext _appDbContext;

        public ChiranSalesRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
