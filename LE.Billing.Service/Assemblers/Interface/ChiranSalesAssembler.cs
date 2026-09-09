using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface ChiranSalesAssembler
    {
        void copy(ChiranSales chiranSales, ChiranSalesDto chiranSalesDto);
    }
}
