using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class WoodBillDetailAssemblerImpl : WoodBillDetailAssembler
    {
        public void copy(WoodBillDetail woodBillDetail, WoodBillDetailDto woodBillDetailDto)
        {
            woodBillDetail.wood_bill_id = woodBillDetailDto.wood_bill_id;
            woodBillDetail.amount = woodBillDetailDto.amount;
            woodBillDetail.rate = woodBillDetailDto.rate;
            woodBillDetail.wood_details_id = woodBillDetailDto.wood_details_id;
        }
    }
}
