using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface ChiranSalesDetailService
    {
        void insert(List<ChiranSalesDetailDto> chiranSalesDetailDtos);
    }
}
