using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Billing.Service.Services.Implementations
{
    public class FurnitureServiceImpl : FurnitureService
    {
        private readonly FurnitureRepository _furnitureRepo;
        private readonly FurnitureAssembler _furnitureAssembler;
        private readonly FurnitureCategoryRepository _furnitureCategoryRepo;

        public FurnitureServiceImpl(FurnitureRepository furnitureRepo, FurnitureAssembler furnitureAssembler, FurnitureCategoryRepository furnitureCategoryRepo)
        {
            _furnitureRepo = furnitureRepo;
            _furnitureAssembler = furnitureAssembler;
            _furnitureCategoryRepo = furnitureCategoryRepo;
        }

        public void delete(long furniture_id)
        {
            try
            {
                using (var tx = _furnitureRepo.beginTransaction())
                {
                    var furniture = _furnitureRepo.getById(furniture_id);

                    if (furniture == null)
                    {
                        throw new ItemNotFoundException($"Furniture with id {furniture_id} does not exist.");
                    }

                    //var salesDetailOfSpecifiedService = _salesDetailRepo.getByTypeAndId(neeldavid.common.Enums.SalesType.service, service_id);

                    //if (salesDetailOfSpecifiedService.Count > 0)
                    //{
                    //    throw new ItemUsedException($"Specified service have already been used in sales.");
                    //}

                    _furnitureRepo.delete(furniture);

                    _furnitureRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void disable(long furniture_id)
        {
            try
            {
                using (var tx = _furnitureRepo.beginTransaction())
                {
                    var furniture = _furnitureRepo.getById(furniture_id);

                    if (furniture == null)
                    {
                        throw new ItemNotFoundException($"Furniture with id {furniture_id} does not exist.");
                    }

                    furniture.disable();
                    _furnitureRepo.update(furniture);

                    _furnitureRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long furniture_id)
        {
            try
            {
                using (var tx = _furnitureRepo.beginTransaction())
                {
                    var furniture = _furnitureRepo.getById(furniture_id);

                    if (furniture == null)
                    {
                        throw new ItemNotFoundException($"Furniture with id {furniture_id} does not exist.");
                    }

                    furniture.enable();
                    _furnitureRepo.update(furniture);

                    _furnitureRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void insert(FurnitureDto furniture_dto)
        {
            try
            {
                using (var tx = _furnitureRepo.beginTransaction())
                {
                    bool isNameValid = checkNameValidity(furniture_dto);

                    if (!isNameValid)
                    {
                        throw new DuplicateItemException("Furniture with same name already exists.");
                    }

                    var furniture = new Furniture();

                    _furnitureAssembler.copy(furniture, furniture_dto);

                    furniture.furnitureCategory = _furnitureCategoryRepo.getById(furniture_dto.furniture_category_id) ?? throw new ItemNotFoundException($"Furniture category with the id {furniture_dto.furniture_id} does not exist.");

                    _furnitureRepo.insert(furniture);

                    _furnitureRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(FurnitureDto furniture_dto)
        {
            try
            {
                using (var tx = _furnitureRepo.beginTransaction())
                {
                    var furniture = _furnitureRepo.getById(furniture_dto.furniture_id);

                    if (furniture == null)
                    {
                        throw new ItemNotFoundException($"Furniture with the id {furniture_dto.furniture_id} does not exist.");
                    }

                    bool isNameValid = checkNameValidity(furniture_dto);

                    if (!isNameValid)
                    {
                        throw new DuplicateItemException("Furniture with same name already exists.");
                    }

                    _furnitureAssembler.copy(furniture, furniture_dto);

                    furniture.furnitureCategory = _furnitureCategoryRepo.getById(furniture_dto.furniture_category_id) ?? throw new ItemNotFoundException($"Furniture category with the id {furniture_dto.furniture_id} does not exist.");

                    _furnitureRepo.update(furniture);

                    _furnitureRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool checkNameValidity(FurnitureDto furniture_dto)
        {
            var furniture = _furnitureRepo.getByName(furniture_dto.name);

            return furniture == null || furniture.furniture_id == furniture_dto.furniture_id;
        }
    }
}
