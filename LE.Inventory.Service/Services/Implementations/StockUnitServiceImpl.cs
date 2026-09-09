using LE.Common.Exceptions;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;
using System.Transactions;

namespace LE.Inventory.Service.Services.Implementations
{
    public class StockUnitServiceImpl:StockUnitService
    {
        private readonly StockUnitRepository _stockUnitRepo;
        private readonly StockUnitAssembler _stockUnitAssembler;

        public StockUnitServiceImpl(StockUnitRepository stockUnitRepo, StockUnitAssembler stockUnitAssembler)
        {
            _stockUnitRepo = stockUnitRepo;
            _stockUnitAssembler = stockUnitAssembler;
        }

        public void delete(long stock_unit_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var stockUnit = _stockUnitRepo.getById(stock_unit_id);

                    if (stockUnit == null)
                        throw new ItemNotFoundException($"The Stock Unit with {stock_unit_id} doesnot exist.");

                    if (stockUnit.hasStockItems())
                        throw new ItemUsedException($"The Stock Unit with id {stock_unit_id} already has stock.You cannot delete at this moment.");

                    _stockUnitRepo.delete(stockUnit);
                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void disable(long stock_unit_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var stockUnit = _stockUnitRepo.getById(stock_unit_id);

                    if (stockUnit == null)
                        throw new ItemNotFoundException($"The Stock Unit with id {stock_unit_id} doesnot exist.");

                    stockUnit.disable();
                    _stockUnitRepo.update(stockUnit);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long stock_unit_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var stockUnit = _stockUnitRepo.getById(stock_unit_id);
                    if (stockUnit == null)
                        throw new ItemNotFoundException($"The Stock Unit with id {stock_unit_id} doesnot exist.");

                    stockUnit.enable();
                    _stockUnitRepo.update(stockUnit);
                    tx.Complete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void save(StockUnitDto stock_unit_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    StockUnit stock_unit = new StockUnit();
                    _stockUnitAssembler.copy(ref stock_unit, stock_unit_dto);
                    _stockUnitRepo.insert(stock_unit);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(StockUnitDto stock_unit_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    StockUnit stockCategory = _stockUnitRepo.getById(stock_unit_dto.stock_unit_id);
                    if (stockCategory == null)
                        throw new ItemNotFoundException($"The Stock Unit with id {stock_unit_dto.stock_unit_id} doesnot exist");

                    _stockUnitAssembler.copy(ref stockCategory, stock_unit_dto);
                    _stockUnitRepo.update(stockCategory);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
