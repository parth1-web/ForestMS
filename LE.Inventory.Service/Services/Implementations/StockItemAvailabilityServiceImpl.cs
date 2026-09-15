using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Collections.Generic;

namespace LE.Inventory.Service.Services.Implementations
{
    public class StockItemAvailabilityServiceImpl : StockItemAvailabilityService
    {
        private readonly StockItemAvailabilityRepository _stockItemAvailabilityRepo;
        private readonly StockItemAvailabilityAssembler _stockItemAvailabilityAssembler;
        private readonly StockItemRepository _stockItemRepo;

        public StockItemAvailabilityServiceImpl(StockItemAvailabilityRepository stockItemAvailabilityRepo, StockItemAvailabilityAssembler stockItemAvailabilityMaker, StockItemRepository stockItemRepo)
        {
            _stockItemAvailabilityRepo = stockItemAvailabilityRepo;
            _stockItemAvailabilityAssembler = stockItemAvailabilityMaker;
            _stockItemRepo = stockItemRepo;
        }


        public void saveOrUpdate(List<StockMovementDetailDto> stock_movement_details)
        {
            // UoW note: update()/delete() are deferred on the shared context, but this
            // loop reads the availability row from the database each iteration. A bill
            // can list the same stock item on several lines, so each iteration must see
            // the previous iteration's write — flush before the next read or duplicate
            // lines would each read the same stale row and double-spend / double-add stock.
            foreach (var stockMovementDetail in stock_movement_details)
            {
                var alreadyRecordedStockItemAvailability = _stockItemAvailabilityRepo.getByStockItemId(stockMovementDetail.stock_item_id);

                if (alreadyRecordedStockItemAvailability == null)
                {
                    // P1/B8 fix: the first movement on a missing availability row used to
                    // store the qty as positive even for decreases, silently creating stock
                    // out of nothing. A decrease with no recorded availability means there
                    // is nothing to sell/burn — reject it.
                    if (stockMovementDetail.operation == LE.Inventory.Common.Enums.MovementOperation.decrease)
                    {
                        throw new ItemUsedException($"Stock item {stockMovementDetail.stock_item_id} has no recorded availability to decrease.");
                    }
                    insert(stockMovementDetail);
                }
                else
                {
                    update(stockMovementDetail, alreadyRecordedStockItemAvailability);
                }
                _stockItemAvailabilityRepo.saveChanges();
            }
        }

        private void update(StockMovementDetailDto stockMovementDetail, StockItemAvailability alreadyRecordedStockItemAvailability)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();

            // P1/B8 fix: no negative-stock guard existed; a read-modify-write could push
            // qty below zero. Decreases now validate the resulting quantity first.
            if (stockMovementDetail.operation == LE.Inventory.Common.Enums.MovementOperation.decrease)
            {
                if (alreadyRecordedStockItemAvailability.qty < stockMovementDetail.qty)
                {
                    throw new ItemUsedException($"Stock item {stockMovementDetail.stock_item_id} has insufficient stock. Available: {alreadyRecordedStockItemAvailability.qty}, requested: {stockMovementDetail.qty}.");
                }
                alreadyRecordedStockItemAvailability.qty -= stockMovementDetail.qty;
            }
            else
            {
                alreadyRecordedStockItemAvailability.qty += stockMovementDetail.qty;
            }

            alreadyRecordedStockItemAvailability.last_updated_date = DateTime.Now;
            alreadyRecordedStockItemAvailability.nep_last_updated_date = dateConverterService.ToBS(DateTime.Now).getFormattedDate();

            _stockItemAvailabilityRepo.update(alreadyRecordedStockItemAvailability);
        }

        private void insert(StockMovementDetailDto stockMovementDetail)
        {
            StockItemAvailabilityDto dto = new StockItemAvailabilityDto();
            dto.stock_item_id = stockMovementDetail.stock_item_id;
            dto.qty = stockMovementDetail.qty;

            StockItemAvailability entity = new StockItemAvailability();

            _stockItemAvailabilityAssembler.copy(entity, dto);
            _stockItemAvailabilityRepo.insert(entity);
        }
    }
}
