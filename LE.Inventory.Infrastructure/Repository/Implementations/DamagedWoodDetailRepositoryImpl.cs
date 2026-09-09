using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class DamagedWoodDetailRepositoryImpl : BaseRepositoryImpl<DamagedWoodDetail>, DamagedWoodDetailRepository
    {
        private readonly AppDbContext _appDbContext;
        public DamagedWoodDetailRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
