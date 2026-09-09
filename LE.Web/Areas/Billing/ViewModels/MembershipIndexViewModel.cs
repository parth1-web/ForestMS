using System.Collections.Generic;
using System;

namespace LE.Web.Areas.Billing.ViewModels
{
	public class MembershipIndexViewModel
	{
		public long MembershipId { get; set; }
		public string MembershipCode { get; set; }
		public string Area { get; set; }
		public string ToleNo { get; set; }
		public string Religion { get; set; }
		public bool HasBioGas { get; set; }
		public bool HasLPG { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsActive { get; set; }
		public string Remarks { get; set; }
		public string CreatedDate { get; set; }
		public string Status { get; set; }
		public List<MembersIndexViewModel> MemberDetails { get; set; }
		public MembershipValidityIndexViewModel MembershipValidity { get; set; }
	}

	public class MembersIndexViewModel
	{
		public string FullName { get; set; }
		public bool IsGharmuli { get; set; }
		public string MemberCitizenship { get; set; }
		public int Age { get; set; }
		public string Gender { get; set; }
		public string Address { get; set; }
		public string ContactNo { get; set; }
		public string FamilyRole { get; set; }
		public string CreatedDate { get; set; }
		public string ImageName { get; set; }
		public string Status { get; set; }
	}

	public class MembershipValidityIndexViewModel
	{
		public string IssueDate { get; set; }
		public DateTime ValidityDate { get; set; }
		public string NepValidityDate { get; set; }
	}
}
