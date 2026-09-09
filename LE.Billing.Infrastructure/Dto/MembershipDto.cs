using LE.Billing.Entities;
using LE.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
	public class MembershipDto
	{
		public long MembershipId { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Membership code is required.")]
		[MaxLength(25)]
		public string MembershipCode { get; set; }
		public string Area { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tole is required.")]
		public string ToleNo { get; set; }
		public ReligionType Religion { get; set; }
		public bool HasBioGas { get; set; } = false;
		public bool HasLPG { get; set; } = true;
		public long CreatedBy { get; set; }
		public bool IsCancelled { get; set; } = false;
		public bool IsActive { get; set; } = true;
		public string Remarks { get; set; }
		public long LedgerId { get; set; }
		public DateTime? CancelledDate { get; set; }
		public string CreatedDate { get; set; }
		public virtual List<Member> MemberDetails { get; set; }
		public virtual List<MemberPunishment> MemberPunishments { get; set; }
		public virtual MembershipValidity MembershipValidity { get; set; }
		bool IsPunished()
		{
			if (MemberPunishments.Count > 0)
			{
				return true;
			}
			return false;
		}
	}
}
