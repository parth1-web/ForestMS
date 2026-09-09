using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface FurnitureSalesAssembler
    {
        void copy(FurnitureSales furniture_sales, FurnitureSalesDto furniture_sales_dto);
    }
}
