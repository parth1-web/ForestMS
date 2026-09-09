using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface ServiceCategoryService
    {
        void insert(ServiceCategoryDto service_category_dto);
        void update(ServiceCategoryDto service_category_dto);
        void delete(long service_category_id);
        void enable(long service_category_id);
        void disable(long service_category_id);
    }
}
