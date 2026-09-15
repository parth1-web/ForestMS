using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Billing.Service.Services.Implementations
{
    public class FurnitureCategoryServiceImpl : FurnitureCategoryService
    {
        private readonly FurnitureCategoryRepository _furnitureCategoryRepo;
        private readonly FurnitureCategoryAssembler _furnitureCategoryAssembler;

        public FurnitureCategoryServiceImpl(FurnitureCategoryRepository furnitureCategoryRepo, FurnitureCategoryAssembler furnitureCategoryAssembler)
        {
            _furnitureCategoryRepo = furnitureCategoryRepo;
            _furnitureCategoryAssembler = furnitureCategoryAssembler;
        }

        public void delete(long furniture_category_id)
        {
            try
            {
                using (var tx = _furnitureCategoryRepo.beginTransaction())
                {
                    var furnitureCategory = _furnitureCategoryRepo.getById(furniture_category_id);

                    if (furnitureCategory == null)
                    {
                        throw new ItemNotFoundException($"Furniture Category with id {furniture_category_id} doesnot exist.");
                    }

                    if (furnitureCategory.hasFurnitures())
                    {
                        throw new ChildCollectionsPresentException($"Furniture have already been assigned to the specified category.");
                    }

                    _furnitureCategoryRepo.delete(furnitureCategory);

                    _furnitureCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void disable(long furniture_category_id)
        {
            try
            {
                using (var tx = _furnitureCategoryRepo.beginTransaction())
                {
                    var furnitureCategory = _furnitureCategoryRepo.getById(furniture_category_id);

                    if (furnitureCategory == null)
                    {
                        throw new ItemNotFoundException($"Customer Category with id {furniture_category_id} doesnot exist.");
                    }

                    furnitureCategory.disable();
                    _furnitureCategoryRepo.update(furnitureCategory);

                    _furnitureCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long furniture_category_id)
        {
            try
            {
                using (var tx = _furnitureCategoryRepo.beginTransaction())
                {
                    var furnitureCategory = _furnitureCategoryRepo.getById(furniture_category_id);

                    if (furnitureCategory == null)
                    {
                        throw new ItemNotFoundException($"Customer Category with id {furniture_category_id} doesnot exist.");
                    }

                    furnitureCategory.enable();
                    _furnitureCategoryRepo.update(furnitureCategory);

                    _furnitureCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void insert(FurnitureCategoryDto furniture_category_dto)
        {
            try
            {
                using (var tx = _furnitureCategoryRepo.beginTransaction())
                {
                    var furnitureCategory = _furnitureCategoryRepo.getByName(furniture_category_dto.name);

                    if (furnitureCategory != null)
                    {
                        throw new DuplicateItemException($"Furniture Category with the same name already exists.");
                    }

                    furnitureCategory = new FurnitureCategory();

                    _furnitureCategoryAssembler.copy(furnitureCategory, furniture_category_dto);

                    _furnitureCategoryRepo.insert(furnitureCategory);

                    _furnitureCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(FurnitureCategoryDto furniture_category_dto)
        {
            try
            {
                using (var tx = _furnitureCategoryRepo.beginTransaction())
                {
                    var furnitureCategory = _furnitureCategoryRepo.getById(furniture_category_dto.furniture_category_id);

                    if (furnitureCategory == null)
                    {
                        throw new ItemNotFoundException($"Furniture Category with the id {furniture_category_dto.furniture_category_id} doesnot exist.");
                    }

                    _furnitureCategoryAssembler.copy(furnitureCategory, furniture_category_dto);

                    _furnitureCategoryRepo.update(furnitureCategory);

                    _furnitureCategoryRepo.saveChanges();
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
