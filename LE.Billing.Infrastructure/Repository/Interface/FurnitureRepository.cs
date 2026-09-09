using LE.Billing.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FurnitureRepository
    {
        void insert(Furniture furniture);
        void update(Furniture furniture);
        void delete(Furniture furniture);
        List<Furniture> getAll();
        Furniture getById(long furniture_id);
        Furniture getByName(string furniture_name);
        IQueryable<Furniture> getQueryable();
    }
}
