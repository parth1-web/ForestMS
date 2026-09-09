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
    public class PilingServiceImpl : PilingService
    {
        private readonly PilingRepository _pilingRepo;
        private readonly PilingAssembler _pilingAssembler;

        public PilingServiceImpl(PilingRepository pilingRepo, PilingAssembler pilingAssembler)
        {
            _pilingRepo = pilingRepo;
            _pilingAssembler = pilingAssembler;
        }

        public void delete(long piling_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var piling = _pilingRepo.getById(piling_id);

                    if (piling == null)
                        throw new ItemNotFoundException($"Piling with {piling_id} not found.");

                    if (piling.hasWoods())
                        throw new ItemUsedException($"The piling with id {piling_id} already has woods. You cannot delete.");

                    _pilingRepo.delete(piling);
                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void disable(long piling_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var piling = _pilingRepo.getById(piling_id);

                    if (piling == null)
                        throw new ItemNotFoundException($"Piling with id {piling_id} not found.");

                    piling.disable();
                    _pilingRepo.update(piling);

                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void enable(long piling_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var piling = _pilingRepo.getById(piling_id);

                    if (piling == null)
                        throw new ItemNotFoundException($"Piling with id {piling_id} not found.");

                    piling.enable();
                    _pilingRepo.update(piling);

                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void save(PilingDto pilingDto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    Piling piling = new Piling();
                    _pilingAssembler.copy(ref piling, pilingDto);
                    _pilingRepo.insert(piling);

                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void update(PilingDto pilingDto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var piling = _pilingRepo.getById(pilingDto.piling_id);

                    if (piling == null)
                        throw new ItemNotFoundException($"The piling with id {pilingDto.piling_id} not found.");

                    _pilingAssembler.copy(ref piling, pilingDto);
                    _pilingRepo.update(piling);
                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
