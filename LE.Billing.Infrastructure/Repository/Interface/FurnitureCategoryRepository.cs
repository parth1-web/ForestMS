using LE.Billing.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FurnitureCategoryRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(FurnitureCategory furniture_category);
        void update(FurnitureCategory furniture_category);
        void delete(FurnitureCategory furniture_category);
        List<FurnitureCategory> getAll();
        FurnitureCategory getById(long furniture_category_id);
        FurnitureCategory getByName(string furniture_category_id);
        IQueryable<FurnitureCategory> getQueryable();
    }
}
