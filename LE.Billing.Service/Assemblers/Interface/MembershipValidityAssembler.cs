using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface MembershipValidityAssembler
    {
        void copy(MembershipValidity membershipPeriod, MembershipValidityDto membershipPeriodDto);
    }
}
