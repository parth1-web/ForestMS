using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface MembershipService
    {
        Membership Insert(MembershipDto membershipDto);
        void Update(MembershipDto membershipDto);
        void CancelMembership(long membershipId);
        void Enable(long membershipId);
        void Disable(long membershipId);
        void Delete(long membershipId);
    }
}
