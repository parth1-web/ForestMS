using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface CounterSalesService
    {
        long makeSales(CounterSalesDto sales_dto);
        void cancel(long counter_sales_id,long user_id);
    }
}
