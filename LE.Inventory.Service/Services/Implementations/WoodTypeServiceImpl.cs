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
    public class WoodTypeServiceImpl:WoodTypeService
    {
        private readonly WoodTypeRepository _woodTypeRepo;
        private readonly WoodTypeAssembler _woodTypeAssembler;

        public WoodTypeServiceImpl(WoodTypeRepository woodTypeRepo, WoodTypeAssembler woodTypeAssembler)
        {
            _woodTypeRepo = woodTypeRepo;
            _woodTypeAssembler = woodTypeAssembler;
        }

        public void delete(long wood_type_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var woodType = _woodTypeRepo.getById(wood_type_id);

                    if (woodType == null)
                        throw new ItemNotFoundException($"The Wood Type with {wood_type_id} doesnot exist.");

                    if (woodType.hasWoodDetails())
                        throw new ItemUsedException($"The Wood Type with id {wood_type_id} already has stock.You cannot delete at this moment.");

                    _woodTypeRepo.delete(woodType);
                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void disable(long wood_type_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var woodType = _woodTypeRepo.getById(wood_type_id);

                    if (woodType == null)
                        throw new ItemNotFoundException($"The Wood Type with id {wood_type_id} doesnot exist.");

                    woodType.disable();
                    _woodTypeRepo.update(woodType);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long wood_type_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var woodType = _woodTypeRepo.getById(wood_type_id);
                    if (woodType == null)
                        throw new ItemNotFoundException($"The Wood Type with id {wood_type_id} doesnot exist.");

                    woodType.enable();
                    _woodTypeRepo.update(woodType);
                    tx.Complete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void save(WoodTypeDto wood_type_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    WoodType wood_type = new WoodType();
                    _woodTypeAssembler.copy(wood_type, wood_type_dto);
                    _woodTypeRepo.insert(wood_type);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(WoodTypeDto wood_type_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    WoodType woodType = _woodTypeRepo.getById(wood_type_dto.wood_type_id);
                    if (woodType == null)
                        throw new ItemNotFoundException($"The Wood Type with id {wood_type_dto.wood_type_id} doesnot exist");

                    _woodTypeAssembler.copy(woodType, wood_type_dto);
                    _woodTypeRepo.update(woodType);
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
