using LE.Account.Entities;
using LE.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Entities
{
	public class Membership
	{
		[Key]
		public long MembershipId { get; set; }
		[Required]
		[MaxLength(25)]
		public string MembershipCode { get; set; }
		public string Area { get; set; }
		public string ToleNo { get; set; }
		public ReligionType Religion { get; set; }
		public bool HasBioGas { get; set; }
		public bool HasLPG { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public long CreatedBy { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsActive { get; set; }
		public string Remarks { get; set; }
		public long LedgerId { get; set; }
		public virtual Ledger ledger { get; set; }
		public DateTime? CancelledDate { get; set; }
		public virtual List<MemberPunishment> MemberPunishments { get; set; }
		public virtual List<Member> MemberDetails { get; set; }
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
