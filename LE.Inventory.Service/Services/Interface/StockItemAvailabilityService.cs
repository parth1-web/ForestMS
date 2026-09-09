using LE.Inventory.Infrastructure.Dto;
using System.Collections.Generic;

namespace LE.Inventory.Service.Services.Interface
{
    public interface StockItemAvailabilityService
    {
        void saveOrUpdate(List<StockMovementDetailDto> stock_movement_details);
    }
}
