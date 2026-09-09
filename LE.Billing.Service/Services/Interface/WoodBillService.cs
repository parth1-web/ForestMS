using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface WoodBillService
    {
        long insert(WoodBillDto wood_bill_dto);
        void cancel(long wood_bill_id, long user_id); 
    }
}
