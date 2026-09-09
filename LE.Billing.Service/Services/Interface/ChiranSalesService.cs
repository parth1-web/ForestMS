using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface ChiranSalesService
    {
        long insert(ChiranSalesDto chiran_sales_id);
        void cancel(long chiran_sales_id,long user_id); 
    }
}
