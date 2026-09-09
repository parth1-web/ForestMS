using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface StockItemAvailabilityAssembler
    {
        void copy(StockItemAvailability stock_item_availability, StockItemAvailabilityDto stock_item_availability_dto);
    }
}
