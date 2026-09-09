using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface MemberAssembler
    {
        void copy(Member member, MemberDto member_dto);
    }
}
