using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class WoodDetailsRepositoryImpl : BaseRepositoryImpl<WoodDetails>, WoodDetailsRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodDetailsRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<WoodDetails> getByGoliaNo(string golia_no, string year)
        {
            return _appDbContext.wood_details.Where(a => a.goliya_number.Contains(golia_no) && a.year.Contains(year)).ToList();
        }
    }
}
