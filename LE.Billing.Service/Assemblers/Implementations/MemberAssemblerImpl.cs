using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
	public class MemberAssemblerImpl : MemberAssembler
	{
		public void copy(Member member, MemberDto memberDto)
		{
			member.MemberId = memberDto.MemberId;
			member.MembershipId = memberDto.MembershipId;
			member.MemberCitizenship = memberDto.MemberCitizenship;
			member.FullName = memberDto.FullName;
			member.Address = memberDto.Address;
			member.ContactNo = memberDto.ContactNo;
			member.Age = memberDto.Age;
			member.Gender = memberDto.Gender;
			member.IsGharmuli = memberDto.IsGharmuli;
			member.CreatedBy = memberDto.CreatedBy;
			member.IsActive = memberDto.IsActive;
			if (!string.IsNullOrEmpty(memberDto.ImageName))
			{
				member.ImageName = memberDto.ImageName;
			}
			member.FamilyRole = memberDto.FamilyRole;
		}
	}
}
