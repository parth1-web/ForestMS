using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class StockMovementRepositoryImpl : BaseRepositoryImpl<StockMovement>, StockMovementRepository
    {
        private readonly AppDbContext _appDbContext;

        public StockMovementRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            appDbContext = _appDbContext;
        }
    }
}
