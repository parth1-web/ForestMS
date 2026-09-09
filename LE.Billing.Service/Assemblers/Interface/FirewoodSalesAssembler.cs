using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface FirewoodSalesAssembler
    {
        void copy(FirewoodSales firewood_sales, FirewoodSalesDto firewood_sales_dto);
    }
}
