using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface WoodBillDetailService
    {
        void insert(List<WoodBillDetailDto> wood_bill_dtos);
    }
}
