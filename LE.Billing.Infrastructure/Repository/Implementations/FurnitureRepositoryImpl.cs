using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class FurnitureRepositoryImpl : BaseRepositoryImpl<Furniture>, FurnitureRepository
    {
        private readonly AppDbContext _appDbContext;

        public FurnitureRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Furniture getByName(string furniture_name)
        {
            return _appDbContext.furniture.Where(a => a.name == furniture_name).SingleOrDefault();
        }
    }
}
