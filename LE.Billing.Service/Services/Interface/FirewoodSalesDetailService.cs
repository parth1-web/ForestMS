using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface FirewoodSalesDetailService
    {
        void save(List<FirewoodSalesDetailDto> firewood_sales_detail_dtos);
    }
}
