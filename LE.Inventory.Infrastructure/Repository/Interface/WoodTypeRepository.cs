using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface WoodTypeRepository
    {
        void insert(WoodType woodType);
        void update(WoodType woodType);
        void delete(WoodType woodType);
        List<WoodType> getAll();
        WoodType getById(long wood_type_id);
        WoodType getByName(string name);
        IQueryable<WoodType> getQueryable();
    }
}
