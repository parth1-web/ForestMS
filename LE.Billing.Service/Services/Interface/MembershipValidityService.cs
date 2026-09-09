using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface MembershipValidityService
    {
        MembershipValidity Insert(MembershipValidityDto membershipDto);
        void Update(MembershipValidityDto membershipDto);
        void Expire(long membershipPeriod_id);
        void Unexpire(long membershipPeriod_id);
        void Cancel(long membershipPeriod_id);
    }
}
