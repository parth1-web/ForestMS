using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface WoodBillMembersAssembler
    {
        void copy(WoodBillMembers woodBillMember, WoodBillMemberDto woodBillMemberDto);
    }
}
