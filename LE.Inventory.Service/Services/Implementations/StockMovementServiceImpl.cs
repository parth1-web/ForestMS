using LE.Common.Exceptions;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Adapter.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Transactions;

namespace LE.Inventory.Service.Services.Implementations
{
    public class StockMovementServiceImpl : StockMovementService
    {
        private readonly StockMovementRepository _stockMovementRepo;
        private readonly StockMovementAssembler _stockMovementAssembler;
        private readonly Movement_ItemAvailabilityAdapter _movement_ItemAvailabilityAdapter;

        public StockMovementServiceImpl(StockMovementRepository stockMovementRepo, StockMovementAssembler stockMovementAssembler, Movement_ItemAvailabilityAdapter movement_ItemAvailabilityAdapter)
        {
            _stockMovementAssembler = stockMovementAssembler;
            _stockMovementRepo = stockMovementRepo;
            _movement_ItemAvailabilityAdapter = movement_ItemAvailabilityAdapter;
        }

        public void record(StockMovementDto stock_movement_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (!stock_movement_dto.isMovementValid())
                    {
                        throw new InvalidValueException("Stock movement data is not valid.");
                    }

                    var stockMovement = new StockMovement();

                    _stockMovementAssembler.copy(stockMovement, stock_movement_dto);

                    _stockMovementRepo.insert(stockMovement);

                    updateItemAvailability(stock_movement_dto);

                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateItemAvailability(StockMovementDto stock_movement_dto)
        {
            _movement_ItemAvailabilityAdapter.updateItemAvailability(stock_movement_dto);
        }
    }
}
