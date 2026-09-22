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
    public class ServiceOfServiceImpl : ServiceOfService
    {
        private readonly ServiceRepository _serviceRepo;
        private readonly ServiceAssembler _serviceAssembler;
        private readonly ServiceCategoryRepository _serviceCategoryRepo;

        public ServiceOfServiceImpl(ServiceRepository serviceRepo, ServiceAssembler serviceAssembler, ServiceCategoryRepository serviceCategoryRepo)
        {
            _serviceRepo = serviceRepo;
            _serviceAssembler = serviceAssembler;
            _serviceCategoryRepo = serviceCategoryRepo;
        }

        public void delete(long service_id)
        {
            try
            {
                using (var tx = _serviceRepo.beginTransaction())
                {
                    var service = _serviceRepo.getById(service_id);

                    if (service == null)
                    {
                        throw new ItemNotFoundException($"Service with id {service_id} does not exist.");
                    }

                    //var salesDetailOfSpecifiedService = _salesDetailRepo.getByTypeAndId(neeldavid.common.Enums.SalesType.service, service_id);

                    //if (salesDetailOfSpecifiedService.Count > 0)
                    //{
                    //    throw new ItemUsedException($"Specified service have already been used in sales.");
                    //}

                    _serviceRepo.delete(service);

                    _serviceRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void disable(long service_id)
        {
            try
            {
                using (var tx = _serviceRepo.beginTransaction())
                {
                    var service = _serviceRepo.getById(service_id);

                    if (service == null)
                    {
                        throw new ItemNotFoundException($"Service with id {service_id} does not exist.");
                    }

                    service.disable();
                    _serviceRepo.update(service);

                    _serviceRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long service_id)
        {
            try
            {
                using (var tx = _serviceRepo.beginTransaction())
                {
                    var service = _serviceRepo.getById(service_id);

                    if (service == null)
                    {
                        throw new ItemNotFoundException($"Service with id {service_id} does not exist.");
                    }

                    service.enable();
                    _serviceRepo.update(service);

                    _serviceRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void insert(ServiceDto service_dto)
        {
            try
            {
                using (var tx = _serviceRepo.beginTransaction())
                {
                    bool isNameValid = checkNameValidity(service_dto);

                    if (!isNameValid)
                    {
                        throw new DuplicateItemException("Service with same name already exists.");
                    }

                    var service = new LE.Billing.Entities.Service();

                    _serviceAssembler.copy(service, service_dto);

                    service.service_category = _serviceCategoryRepo.getById(service_dto.category_id) ?? throw new ItemNotFoundException($"Service category with the id {service_dto.category_id} does not exist.");

                    _serviceRepo.insert(service);

                    _serviceRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void update(ServiceDto service_dto)
        {
            try
            {
                using (var tx = _serviceRepo.beginTransaction())
                {
                    var service = _serviceRepo.getById(service_dto.service_id);

                    if (service == null)
                    {
                        throw new ItemNotFoundException($"Service with the id {service_dto.service_id} does not exist.");
                    }

                    bool isNameValid = checkNameValidity(service_dto);

                    if (!isNameValid)
                    {
                        throw new DuplicateItemException("Service with same name already exists.");
                    }

                    _serviceAssembler.copy(service, service_dto);

                    service.service_category = _serviceCategoryRepo.getById(service_dto.category_id) ?? throw new ItemNotFoundException($"Service category with the id {service_dto.category_id} does not exist.");

                    _serviceRepo.update(service);

                    _serviceRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool checkNameValidity(ServiceDto service_dto)
        {
            var service = _serviceRepo.getByName(service_dto.name);

            return service == null || service.service_id == service_dto.service_id;
        }
    }
}
