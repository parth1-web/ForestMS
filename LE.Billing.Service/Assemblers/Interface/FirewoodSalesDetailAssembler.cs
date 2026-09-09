using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface FirewoodSalesDetailAssembler
    {
        void copy(FirewoodSalesDetail firewood_sales_detail, FirewoodSalesDetailDto firewood_sales_detail_dto);
    }
}
