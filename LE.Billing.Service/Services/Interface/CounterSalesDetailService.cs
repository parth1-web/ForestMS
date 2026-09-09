using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface CounterSalesDetailService
    {
        void save(List<CounterSalesDetailDto> counter_sales_detail_dtos);
    }
}
