using LE.Common.Exceptions;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Adapter.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;

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
            try
            {
                using (TransactionScope tx=new TransactionScope(TransactionScopeOption.Required))
                {
                    List<StockMovementDetailDto> stockMovementDatas = getStockMovementDetails(stock_movement_dto);

                    _stockItemAvailabilityService.saveOrUpdate(stockMovementDatas);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<StockMovementDetailDto> getStockMovementDetails(StockMovementDto stock_movement_dto)
        {
            switch (stock_movement_dto.movement_type)
            {
               
                case Enums.StockMovementType.sales:
                    stock_movement_dto.getStockMovementDetails().ForEach(a => a.operation = Enums.MovementOperation.decrease);
                    break;
                case Enums.StockMovementType.purchase:
                    stock_movement_dto.getStockMovementDetails().ForEach(a => a.operation = Enums.MovementOperation.increase);
                    break;
                case Enums.StockMovementType.delete:
                    stock_movement_dto.getStockMovementDetails().ForEach(a => a.operation = Enums.MovementOperation.increase);
                    break;
                default:
                    throw new InvalidValueException("Invalid stock movement type.");

            }

            return stock_movement_dto.getStockMovementDetails();

        }
    }
}
