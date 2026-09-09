using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface FurnitureSalesDetailAssembler
    {
        void copy(FurnitureSalesDetail furniture_sales_detail, FurnitureSalesDetailDto furniture_sales_detail_dto);
    }
}
