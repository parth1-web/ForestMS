using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface StockMovementService
    {
        void record(StockMovementDto stock_movement_dto);
    }
}
