using LE.Inventory.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface DamagedWoodDetailRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(DamagedWoodDetail damagedWoodDetail);
        void update(DamagedWoodDetail damagedWoodDetail);
        void delete(DamagedWoodDetail damagedWoodDetail);
        List<DamagedWoodDetail> getAll();
        DamagedWoodDetail getById(long damaged_wood_detail_id);
        IQueryable<DamagedWoodDetail> getQueryable();
    }
}
