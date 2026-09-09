using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class StockItemRepositoryImpl : BaseRepositoryImpl<StockItem>, StockItemRepository
    {
        private readonly AppDbContext _appDbContext;
        public StockItemRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
