using DateConverter.Core.Service_Factory;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;

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
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var stockMovementDetail in stock_movement_details)
                    {
                        var alreadyRecordedStockItemAvailability = _stockItemAvailabilityRepo.getByStockItemId(stockMovementDetail.stock_item_id);

                        if (alreadyRecordedStockItemAvailability == null)
                        {
                            insert(stockMovementDetail);
                        }
                        else
                        {
                            update(stockMovementDetail, alreadyRecordedStockItemAvailability);
                        }
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void update(StockMovementDetailDto stockMovementDetail, StockItemAvailability alreadyRecordedStockItemAvailability)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();

            alreadyRecordedStockItemAvailability.last_updated_date = DateTime.Now;
            alreadyRecordedStockItemAvailability.nep_last_updated_date = dateConverterService.ToBS(DateTime.Now).getFormattedDate();

            if (stockMovementDetail.operation ==LE.Inventory.Common.Enums.MovementOperation.decrease)
            {
                alreadyRecordedStockItemAvailability.qty -= stockMovementDetail.qty;
            }
            else
            {
                alreadyRecordedStockItemAvailability.qty += stockMovementDetail.qty;
            }

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
