using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface CounterSalesAssembler
    {
        void copy(CounterSales counter_sales, CounterSalesDto counter_sales_dto);
    }
}
