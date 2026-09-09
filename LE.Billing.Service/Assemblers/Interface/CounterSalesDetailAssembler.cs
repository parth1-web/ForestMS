using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface CounterSalesDetailAssembler
    {
        void copy(CounterSalesDetail counter_sales_detail, CounterSalesDetailDto counter_sales_detail_dto);
    }
}
