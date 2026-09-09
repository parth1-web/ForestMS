using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface MembershipAssembler
    {
        void copy(Membership membership, MembershipDto membershipDto);
    }
}
