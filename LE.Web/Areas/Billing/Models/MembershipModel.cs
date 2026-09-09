using LE.Billing.Entities;
using LE.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LE.Web.Areas.Billing.Models
{
	public class MembershipModel
	{
		public long MembershipId { get; set; }
		[DisplayName("Membership Code")]
		public string MembershipCode { get; set; }
		[DisplayName("Area")]
		public string Area { get; set; }
		[DisplayName("Tole No")]
		public string ToleNo { get; set; }
		[DisplayName("Religion")]
		public ReligionType Religion { get; set; }
		[DisplayName("Has Bio Gas")]
		public bool HasBioGas { get; set; }
		[DisplayName("Has L.P.G")]
		public bool HasLPG { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsActive { get; set; }
		public string Remarks { get; set; }	
		public DateTime CreatedDate{ get; set; }
		public long CreatedBy { get; set; }
		public long LedgerId { get; set; }
		public DateTime? CancelledDate { get; set; }
		public virtual List<MemberModel> MemberDetails { get; set; }
		public virtual MembershipValidityModel MembershipValidity { get; set; }
		public virtual List<MemberPunishment> MemberPunishments { get; set; }
	}

	public class RenewMembershipModel
	{
		public long MembershipId { get; set; }
		public string NepValidityDate { get; set; }
	}
}
