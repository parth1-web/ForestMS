using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class StockItemAvailabilityRepositoryImpl : BaseRepositoryImpl<StockItemAvailability>, StockItemAvailabilityRepository
    {
        private readonly AppDbContext _appDbContext;

        public StockItemAvailabilityRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public StockItemAvailability getByStockItemId(long stock_item_id)
        {
            return _appDbContext.stock_item_availability.Where(a => a.stock_item_id == stock_item_id).SingleOrDefault();
        }
    }
}
