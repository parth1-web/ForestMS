using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Common.Exceptions;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class ServiceAssemblerImpl : ServiceAssembler
    {
        private readonly ServiceCategoryRepository _serviceCategoryRepo;

        public ServiceAssemblerImpl(ServiceCategoryRepository serviceCategoryRepo)
        {
            _serviceCategoryRepo = serviceCategoryRepo;
        }

        public void copy(LE.Billing.Entities.Service service, ServiceDto service_dto)
        {
            service.category_id = service_dto.category_id;
            service.name = service_dto.name;
            service.rate = service_dto.rate;
            service.created_by = service_dto.created_by;
            service.created_date = service_dto.created_date;
            service.is_enabled = service_dto.is_enabled;
            service.ledger_id = service_dto.ledger_id;
            service.tax = service_dto.tax;
            service.service_category = _serviceCategoryRepo.getById(service_dto.category_id) ?? throw new ItemNotFoundException($"Service category with the id {service_dto.category_id} does not exist.");
        }
    }
}
