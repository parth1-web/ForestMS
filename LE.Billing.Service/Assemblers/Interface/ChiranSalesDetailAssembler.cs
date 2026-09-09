using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface ChiranSalesDetailAssembler
    {
        void copy(ChiranSalesDetail chiranSalesDetail, ChiranSalesDetailDto chiranSalesDetailDto);
    }
}
