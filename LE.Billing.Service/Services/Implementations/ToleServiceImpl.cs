using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Transactions;

namespace LE.Billing.Service.Services.Implementations
{
    public class ToleServiceImpl : ToleService
    {
        private readonly ToleRepository _toleRepo;
        private readonly ToleAssembler _toleAssembler;

        public ToleServiceImpl(ToleRepository toleRepo, ToleAssembler toleAssembler)
        {
            _toleRepo = toleRepo;
            _toleAssembler = toleAssembler;
        }

        public void delete(long tole_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var tole = _toleRepo.getById(tole_id);

                    if (tole == null)
                    {
                        throw new ItemNotFoundException($"Tole with id {tole_id} doesnot exist.");
                    }

                    _toleRepo.delete(tole);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void insert(ToleDto tole_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var tole = _toleRepo.getByToleNo(tole_dto.tole_no);

                    if (tole != null)
                    {
                        throw new DuplicateItemException($"Tole No with the same Number already exists.");
                    }

                    tole = new Tole();

                    _toleAssembler.copy(tole, tole_dto);

                    _toleRepo.insert(tole);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(ToleDto tole_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var tole = _toleRepo.getById(tole_dto.tole_id);

                    if (tole == null)
                    {
                        throw new ItemNotFoundException($"Tole with the id {tole_dto.tole_id} doesnot exist.");
                    }

                    if (tole != null)
                    {
                        var toleNo = _toleRepo.getByToleNo(tole_dto.tole_no);
                        if (toleNo != null)
                        {
                            throw new DuplicateItemException($"Tole No with the same Number already exists.");
                        }
                        _toleAssembler.copy(tole, tole_dto);

                        _toleRepo.update(tole);

                        tx.Complete();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
