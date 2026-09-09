using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface WoodBillAssembler
    {
        void copy(WoodBill woodBill, WoodBillDto woodBillDto);
    }
}
