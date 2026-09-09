using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface ServiceAssembler
    {
        void copy(LE.Billing.Entities.Service service, ServiceDto service_dto);
    }
}
