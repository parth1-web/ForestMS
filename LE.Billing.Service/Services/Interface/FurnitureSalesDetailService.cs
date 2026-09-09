using LE.Billing.Infrastructure.Dto;
using System.Collections.Generic;

namespace LE.Billing.Service.Services.Interface
{
    public interface FurnitureSalesDetailService
    {
        void save(List<FurnitureSalesDetailDto> furniture_sales_detail_dtos);
    }
}
