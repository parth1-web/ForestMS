using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface FurnitureSalesService
    {
        long makeSales(FurnitureSalesDto sales_dto);

        void cancel(long furniture_sales_id, long user_id);

    }
}
