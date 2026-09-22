using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Implementations
{
    public class ServiceCategoryServiceImpl : ServiceCategoryService
    {
        private readonly ServiceCategoryRepository _serviceCategoryRepo;
        private readonly ServiceCategoryAssembler _serviceCategoryAssembler;

        public ServiceCategoryServiceImpl(ServiceCategoryRepository serviceCategoryRepo, ServiceCategoryAssembler serviceCategoryAssembler)
        {
            _serviceCategoryRepo = serviceCategoryRepo;
            _serviceCategoryAssembler = serviceCategoryAssembler;
        }

        public void delete(long service_category_id)
        {
            try
            {
                using (var tx = _serviceCategoryRepo.beginTransaction())
                {
                    var serviceCategory = _serviceCategoryRepo.getById(service_category_id);

                    if (serviceCategory == null)
                    {
                        throw new ItemNotFoundException($"Service Category with id {service_category_id} does not exist.");
                    }

                    if (serviceCategory.hasServices())
                    {
                        throw new ChildCollectionsPresentException($"Services have already been assigned to the specified category.");
                    }

                    _serviceCategoryRepo.delete(serviceCategory);

                    _serviceCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void disable(long service_category_id)
        {
            try
            {
                using (var tx = _serviceCategoryRepo.beginTransaction())
                {
                    var serviceCategory = _serviceCategoryRepo.getById(service_category_id);

                    if (serviceCategory == null)
                    {
                        throw new ItemNotFoundException($"Customer Category with id {service_category_id} does not exist.");
                    }

                    serviceCategory.disable();
                    _serviceCategoryRepo.update(serviceCategory);

                    _serviceCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long service_category_id)
        {
            try
            {
                using (var tx = _serviceCategoryRepo.beginTransaction())
                {
                    var serviceCategory = _serviceCategoryRepo.getById(service_category_id);

                    if (serviceCategory == null)
                    {
                        throw new ItemNotFoundException($"Customer Category with id {service_category_id} does not exist.");
                    }

                    serviceCategory.enable();
                    _serviceCategoryRepo.update(serviceCategory);

                    _serviceCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void insert(ServiceCategoryDto service_category_dto)
        {
            try
            {
                using (var tx = _serviceCategoryRepo.beginTransaction())
                {
                    var serviceCategory = _serviceCategoryRepo.getByName(service_category_dto.name);

                    if (serviceCategory != null)
                    {
                        throw new DuplicateItemException($"Service Category with the same name already exists.");
                    }

                    serviceCategory = new ServiceCategory();

                    _serviceCategoryAssembler.copy(serviceCategory, service_category_dto);

                    _serviceCategoryRepo.insert(serviceCategory);

                    _serviceCategoryRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(ServiceCategoryDto service_category_dto)
        {
            try
            {
                using (var tx = _serviceCategoryRepo.beginTransaction())
                {
                    var serviceCategory = _serviceCategoryRepo.getById(service_category_dto.category_id);

                    if (serviceCategory == null)
                    {
                        throw new ItemNotFoundException($"Service Category with the id {service_category_dto.category_id} does not exist.");
                    }

                    _serviceCategoryAssembler.copy(serviceCategory, service_category_dto);

                    _serviceCategoryRepo.update(serviceCategory);

                    _serviceCategoryRepo.saveChanges();
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
