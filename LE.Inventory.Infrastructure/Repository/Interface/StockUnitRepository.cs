using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface StockUnitRepository
    {
        void insert(StockUnit stock_unit);
        void update(StockUnit stock_unit);
        void delete(StockUnit stock_unit);
        List<StockUnit> getAll();
        StockUnit getById(long stock_unit_id);
        IQueryable<StockUnit> getQueryable();
    }
}
