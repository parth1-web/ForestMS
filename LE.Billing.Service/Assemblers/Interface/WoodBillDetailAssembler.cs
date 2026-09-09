using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface WoodBillDetailAssembler
    {
        void copy(WoodBillDetail woodBillDetail, WoodBillDetailDto woodBillDetailDto);
    }
}
