using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface StockMovementRepository
    {
        void insert(StockMovement stock_movement);
        List<StockMovement> getAll();
        StockMovement getById(long stock_movement_id);
        IQueryable<StockMovement> getQueryable();
    }
}
