using LE.Common.Exceptions;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;

namespace LE.Inventory.Service.Services.Implementations
{
    public class StockItemServiceImpl : StockItemService
    {
        private readonly StockItemRepository _stockItemRepo;
        private readonly StockItemAvailabilityRepository _stockItemAvailabilityRepo;
        private readonly StockItemAssembler _stockItemAssembler;
        private readonly StockItemAvailabilityAssembler _stockItemAvailabilityAssembler;

        public StockItemServiceImpl(StockItemRepository stockItemRepo, StockItemAssembler stockItemAssembler, StockItemAvailabilityRepository stockItemAvailabilityRepo, StockItemAvailabilityAssembler stockItemAvailabilityAssembler)
        {
            _stockItemAvailabilityRepo = stockItemAvailabilityRepo;
            _stockItemRepo = stockItemRepo;
            _stockItemAssembler = stockItemAssembler;
            _stockItemAvailabilityAssembler = stockItemAvailabilityAssembler;
        }

        public void delete(long stock_item_id)
        {
            try
            {
                using (var tx = _stockItemRepo.beginTransaction())
                {
                    var stockItem = _stockItemRepo.getById(stock_item_id);

                    if (stockItem == null)
                        throw new ItemNotFoundException($"The Stock Item with {stock_item_id} does not exist.");

                    if (stockItem.hasPurchases())
                        throw new ItemUsedException($"The Stock Item with id {stock_item_id} already has purchases .You cannot delete at this moment.");
                    _stockItemRepo.delete(stockItem);
                    _stockItemRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void disable(long stock_item_id)
        {
            try
            {
                using (var tx = _stockItemRepo.beginTransaction())
                {
                    var stockItem = _stockItemRepo.getById(stock_item_id);

                    if (stockItem == null)
                        throw new ItemNotFoundException($"The Stock Item with id {stock_item_id} does not exist.");

                    stockItem.disable();
                    _stockItemRepo.update(stockItem);

                    _stockItemRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long stock_item_id)
        {
            try
            {
                using (var tx = _stockItemRepo.beginTransaction())
                {
                    var stockItem = _stockItemRepo.getById(stock_item_id);
                    if (stockItem == null)
                        throw new ItemNotFoundException($"The Stock Unit with id {stock_item_id} does not exist.");

                    stockItem.enable();
                    _stockItemRepo.update(stockItem);
                    _stockItemRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void save(StockItemDto stock_item_dto)
        {
            try
            {
                using (var tx = _stockItemRepo.beginTransaction())
                {
                    StockItem stock_item = new StockItem();
                    _stockItemAssembler.copy(ref stock_item, stock_item_dto);
                    _stockItemRepo.insert(stock_item);

                    StockItemAvailability stockItemAvailability = new StockItemAvailability();
                    StockItemAvailabilityDto dto = new StockItemAvailabilityDto
                         ();
                    dto.last_updated_date = DateTime.Now;
                    dto.qty = 0;
                    dto.stock_item_id = stock_item.stock_item_id;
                    _stockItemAvailabilityAssembler.copy(stockItemAvailability, dto);
                    _stockItemAvailabilityRepo.insert(stockItemAvailability);
                    _stockItemRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(StockItemDto stock_item_dto)
        {
            try
            {
                using (var tx = _stockItemRepo.beginTransaction())
                {
                    StockItem stockItem = _stockItemRepo.getById(stock_item_dto.stock_item_id);
                    if (stockItem == null)
                        throw new ItemNotFoundException($"The Stock Item with id {stock_item_dto.stock_item_id} does not exist");

                    _stockItemAssembler.copy(ref stockItem, stock_item_dto);
                    _stockItemRepo.update(stockItem);
                    _stockItemRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
