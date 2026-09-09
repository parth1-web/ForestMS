using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface ServiceOfService
    {
        void enable(long service_id);
        void disable(long service_id);
        void delete(long service_id);
        void insert(ServiceDto service_dto);
        void update(ServiceDto service_dto);
    }
}
