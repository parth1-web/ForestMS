using LE.Inventory.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface StockCategoryPurposeRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(StockCategoryPurpose purpose);
        void update(StockCategoryPurpose purpose);
        void delete(StockCategoryPurpose purpose);
        List<StockCategoryPurpose> getAll();
        StockCategoryPurpose getById(long purpose_id);
        StockCategoryPurpose getByName(string name);
        IQueryable<StockCategoryPurpose> getQueryable();
    }
}
