using LE.Common.Exceptions;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;

namespace LE.Inventory.Service.Services.Implementations
{
    public class StockCategoryPurposeServiceImpl:StockCategoryPurposeService
    {
        private readonly StockCategoryPurposeRepository _stockCategoryPurposeRepo;
        private readonly StockCategoryPurposeAssembler _stockCategoryPurposeAssembler;

        public StockCategoryPurposeServiceImpl(StockCategoryPurposeRepository ballaBalliCategoryRepo, StockCategoryPurposeAssembler ballaBalliCategoryAssembler)
        {
            _stockCategoryPurposeRepo = ballaBalliCategoryRepo;
            _stockCategoryPurposeAssembler = ballaBalliCategoryAssembler;
        }

        public void delete(long stock_category_purpose_id)
        {
            try
            {
                using (var tx = _stockCategoryPurposeRepo.beginTransaction())
                {
                    var stockCategoryPurpose = _stockCategoryPurposeRepo.getById(stock_category_purpose_id);

                    if (stockCategoryPurpose == null)
                        throw new ItemNotFoundException($"The Purpose Category with {stock_category_purpose_id} does not exist.");

                    if (stockCategoryPurpose.hasWoodDetails())
                        throw new ItemUsedException($"The Purpose Category with id {stock_category_purpose_id} already has Woods.You cannot delete at this moment.");

                    _stockCategoryPurposeRepo.delete(stockCategoryPurpose);
                    _stockCategoryPurposeRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void disable(long stock_category_purpose_id)
        {
            try
            {
                using (var tx = _stockCategoryPurposeRepo.beginTransaction())
                {
                    var ballaballiCategory = _stockCategoryPurposeRepo.getById(stock_category_purpose_id);

                    if (ballaballiCategory == null)
                        throw new ItemNotFoundException($"The Purpose Category with id {stock_category_purpose_id} does not exist.");

                    ballaballiCategory.disable();
                    _stockCategoryPurposeRepo.update(ballaballiCategory);

                    _stockCategoryPurposeRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long stock_category_purpose_id)
        {
            try
            {
                using (var tx = _stockCategoryPurposeRepo.beginTransaction())
                {
                    var stockCategoryPurpose = _stockCategoryPurposeRepo.getById(stock_category_purpose_id);
                    if (stockCategoryPurpose == null)
                        throw new ItemNotFoundException($"The Purpose Category with id {stock_category_purpose_id} does not exist.");

                    stockCategoryPurpose.enable();
                    _stockCategoryPurposeRepo.update(stockCategoryPurpose);
                    _stockCategoryPurposeRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void save(StockCategoryPurposeDto stock_category_purpose_dto)
        {
            try
            {
                using (var tx = _stockCategoryPurposeRepo.beginTransaction())
                {
                    StockCategoryPurpose stock_category_purpose = new StockCategoryPurpose();
                    _stockCategoryPurposeAssembler.copy(stock_category_purpose, stock_category_purpose_dto);
                    _stockCategoryPurposeRepo.insert(stock_category_purpose);
                    _stockCategoryPurposeRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(StockCategoryPurposeDto stock_category_purpose_dto)
        {
            try
            {
                using (var tx = _stockCategoryPurposeRepo.beginTransaction())
                {
                    StockCategoryPurpose stock_category_purpose = _stockCategoryPurposeRepo.getById(stock_category_purpose_dto.stock_category_purpose_id);
                    if (stock_category_purpose == null)
                        throw new ItemNotFoundException($"The Purpose Category with id {stock_category_purpose_dto.stock_category_purpose_id} does not exist");

                    _stockCategoryPurposeAssembler.copy(stock_category_purpose, stock_category_purpose_dto);
                    _stockCategoryPurposeRepo.update(stock_category_purpose);
                    _stockCategoryPurposeRepo.saveChanges();
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
