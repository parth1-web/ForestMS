using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface MemberService
    {
        Member Insert(MemberDto member_dto);
        void Update(MemberDto member_dto);
        void Enable(long member_id);
        void Disable(long member_id);
        void Delete(long member_id);
    }
}
