using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class PilingRepositoryImpl : BaseRepositoryImpl<Piling>, PilingRepository
    {
        private readonly AppDbContext _appDbContext;

        public PilingRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
