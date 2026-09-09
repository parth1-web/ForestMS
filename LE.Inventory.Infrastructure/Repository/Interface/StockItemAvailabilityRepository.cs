using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface StockItemAvailabilityRepository
    {
        void insert(StockItemAvailability stock_item_availability);
        void update(StockItemAvailability stock_item_availability);
        List<StockItemAvailability> getAll();
        StockItemAvailability getById(long stock_item_availability_id);
        StockItemAvailability getByStockItemId(long stock_item_id);
        IQueryable<StockItemAvailability> getQueryable();
    }
}
