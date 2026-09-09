using LE.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface WoodDetailsRepository
    {
        void insert(WoodDetails woodDetails);
        void update(WoodDetails woodDetails);
        void delete(WoodDetails woodDetails);
        List<WoodDetails> getAll();
        WoodDetails getById(long wood_details_id);
        IQueryable<WoodDetails> getQueryable();
        List<WoodDetails> getByGoliaNo(string golia_no, string year);
    }
}
