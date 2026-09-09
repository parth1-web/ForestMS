using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class FurnitureCategoryRepositoryImpl : BaseRepositoryImpl<FurnitureCategory>, FurnitureCategoryRepository
    {
        private readonly AppDbContext _appDbContext;

        public FurnitureCategoryRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public FurnitureCategory getByName(string furniture_category_name)
        {
            return _appDbContext.furniture_category.Where(a => a.name == furniture_category_name).SingleOrDefault();
        }
    }
}
