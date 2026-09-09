using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Adapter.Interface
{
    public interface Movement_ItemAvailabilityAdapter
    {
        void updateItemAvailability(StockMovementDto stock_movement_dto);
    }
}
