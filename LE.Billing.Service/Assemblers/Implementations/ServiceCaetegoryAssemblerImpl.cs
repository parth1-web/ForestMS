using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class ServiceCaetegoryAssemblerImpl: ServiceCategoryAssembler
    {
        public void copy(ServiceCategory service_category, ServiceCategoryDto service_category_dto)
        {
            service_category.name = service_category_dto.name;
            service_category.is_enabled = service_category_dto.is_enabled;
        }
    }
}
