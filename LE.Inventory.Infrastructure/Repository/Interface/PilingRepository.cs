using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface PilingRepository
    {
        void insert(Piling piling);
        void update(Piling piling);
        void delete(Piling piling);
        List<Piling> getAll();
        Piling getById(long piling_id);
        IQueryable<Piling> getQueryable();
    }
}
