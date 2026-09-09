using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface FirewoodSalesService
    {
        long makeSales(FirewoodSalesDto sales_dto);

        void cancel(long firewood_sales_id, long user_id);

    }
}
