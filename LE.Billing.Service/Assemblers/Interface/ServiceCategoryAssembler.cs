using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface ServiceCategoryAssembler
    {
        void copy(ServiceCategory service_category, ServiceCategoryDto service_category_dto);
    }
}
