using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class StockUnitRepositoryImpl : BaseRepositoryImpl<StockUnit>, StockUnitRepository
    {
        private readonly AppDbContext _appDbContext;
        public StockUnitRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
