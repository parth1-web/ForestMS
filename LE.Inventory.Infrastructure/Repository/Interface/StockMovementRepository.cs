using LE.Inventory.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface StockMovementRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(StockMovement stock_movement);
        List<StockMovement> getAll();
        StockMovement getById(long stock_movement_id);
        IQueryable<StockMovement> getQueryable();
    }
}
