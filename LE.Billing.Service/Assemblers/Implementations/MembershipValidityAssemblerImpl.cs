using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class MembershipValidityAssemblerImpl : MembershipValidityAssembler
    {
        public void copy(MembershipValidity memPeriod, MembershipValidityDto memPeriodDto)
        {
            memPeriod.MembershipValidityId = memPeriodDto.MembershipValidityId;
            memPeriod.MembershipId = memPeriodDto.MembershipId;
            memPeriod.IssueDate = memPeriodDto.IssueDate;
            memPeriod.ValidityDate = memPeriodDto.ValidityDate;
            memPeriod.NepValidityDate = memPeriodDto.NepValidityDate;
			memPeriod.RenewedDate = memPeriodDto.RenewedDate;
			memPeriod.IsExpired = memPeriodDto.IsExpired;
			memPeriod.IsCurrent = memPeriodDto.IsCurrent;
			memPeriod.IsCancelled = memPeriodDto.IsCancelled;
			memPeriod.CreatedDate = memPeriodDto.CreatedDate;
            memPeriod.CreatedBy = memPeriodDto.CreatedBy;
        }
    }
}
