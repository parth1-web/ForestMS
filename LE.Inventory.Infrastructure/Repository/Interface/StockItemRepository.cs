using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface StockItemRepository
    {
        void insert(StockItem stock_item);
        void update(StockItem stock_item);
        void delete(StockItem stock_item);
        List<StockItem> getAll();
        StockItem getById(long stock_item_id);
        IQueryable<StockItem> getQueryable();
    }
}
