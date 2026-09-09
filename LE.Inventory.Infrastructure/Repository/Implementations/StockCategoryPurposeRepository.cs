using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class StockCategoryPurposeRepositoryImpl : BaseRepositoryImpl<StockCategoryPurpose>, StockCategoryPurposeRepository
    {
        private readonly AppDbContext _appDbContext;

        public StockCategoryPurposeRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public StockCategoryPurpose getByName(string name)
        {
            return _appDbContext.purpose_category.Where(a => a.name == name).SingleOrDefault();
        }
    }
}
