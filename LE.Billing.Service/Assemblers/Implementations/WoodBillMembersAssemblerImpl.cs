using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class WoodBillMembersAssemblerImpl : WoodBillMembersAssembler
    {
        public void copy(WoodBillMembers woodBillMember, WoodBillMemberDto woodBillMemberDto)
        {
            woodBillMember.wood_bill_id = woodBillMemberDto.wood_bill_id;
            woodBillMember.member_id = woodBillMemberDto.member_id;
            
        }
    }
}
