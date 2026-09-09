using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class ChiranSalesDetailAssemblerImpl :ChiranSalesDetailAssembler
    {
        public void copy(ChiranSalesDetail chiranSalesDetail, ChiranSalesDetailDto chiranSalesDetailDto)
        {
            chiranSalesDetail.chiran_sales_id = chiranSalesDetailDto.chiran_sales_id;
            chiranSalesDetail.circle_size = chiranSalesDetailDto.circle_size;
            chiranSalesDetail.length
                = chiranSalesDetailDto.length;
            chiranSalesDetail.breadth = chiranSalesDetailDto.breadth;
            chiranSalesDetail.quantity = chiranSalesDetailDto.quantity;
            chiranSalesDetail.rate = chiranSalesDetailDto.rate;
            chiranSalesDetail.amount = chiranSalesDetailDto.amount;
            chiranSalesDetail.wood_type_id = chiranSalesDetailDto.wood_type_id;
            chiranSalesDetail.total_size = chiranSalesDetailDto.total_size;
        }
    }
}
