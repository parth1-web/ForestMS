using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
	public class MembershipAssemblerImpl : MembershipAssembler
	{
		public void copy(Membership membership, MembershipDto membershipDto)
		{
			membership.MembershipId = membershipDto.MembershipId;
			membership.MembershipCode = membershipDto.MembershipCode;
			membership.Area = membershipDto.Area;
			membership.ToleNo = membershipDto.ToleNo;
			membership.Religion = membershipDto.Religion;
			membership.HasBioGas = membershipDto.HasBioGas;
			membership.HasLPG = membershipDto.HasLPG;
			membership.IsCancelled = membershipDto.IsCancelled;
			membership.CancelledDate = membershipDto.CancelledDate;
			membership.IsActive = membershipDto.IsActive;
			membership.Remarks = membershipDto.Remarks;
			membership.LedgerId = membershipDto.LedgerId;
			membership.CreatedBy = membershipDto.CreatedBy;
		}
	}
}
