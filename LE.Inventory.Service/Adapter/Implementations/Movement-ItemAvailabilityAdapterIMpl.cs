using LE.Common.Exceptions;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Adapter.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Collections.Generic;

namespace LE.Inventory.Service.Adapter.Implementations
{
    using Enums = LE.Inventory.Common.Enums;
    public class Movement_ItemAvailabilityAdapterImpl : Movement_ItemAvailabilityAdapter
    {
        private readonly StockItemAvailabilityService _stockItemAvailabilityService;

        public Movement_ItemAvailabilityAdapterImpl(StockItemAvailabilityService stockItemAvailabilityService)
        {
            _stockItemAvailabilityService = stockItemAvailabilityService;
        }


        public void updateItemAvailability(StockMovementDto stock_movement_dto)
        {
            List<StockMovementDetailDto> stockMovementDatas = getStockMovementDetails(stock_movement_dto);

            _stockItemAvailabilityService.saveOrUpdate(stockMovementDatas);
        }

        private List<StockMovementDetailDto> getStockMovementDetails(StockMovementDto stock_movement_dto)
        {
            // P1/B7 fix: the 'delete' movement type used to be force-mapped to 'increase',
            // which made deleting a purchase INCREASE stock. The service layer already
            // sets the correct operation per detail (purchase -> increase, delete ->
            // decrease, sales -> decrease), so the mapping only applies the default for
            // sales when no explicit operation was set and no longer overrides delete.
            switch (stock_movement_dto.movement_type)
            {
                case Enums.StockMovementType.sales:
                    stock_movement_dto.getStockMovementDetails().ForEach(a => a.operation = Enums.MovementOperation.decrease);
                    break;
                case Enums.StockMovementType.purchase:
                    stock_movement_dto.getStockMovementDetails().ForEach(a => a.operation = Enums.MovementOperation.increase);
                    break;
                case Enums.StockMovementType.delete:
                    // operation is already set by the caller (delete = decrease);
                    // keep it as-is.
                    break;
                default:
                    throw new InvalidValueException("Invalid stock movement type.");

            }

            return stock_movement_dto.getStockMovementDetails();

        }
    }
}
