using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Billing.Models
{
	public class MembershipValidityModel
	{
		public long MembershipValidityId { get; set; }
		public long MembershipId { get; set; }
		[DisplayName("Issue Date")]
		public string IssueDate { get; set; }
		[DisplayName("Validity Date")]
		public string NepValidityDate { get; set; }
		public DateTime? RenewedDate { get; set; }
		public bool IsExpired { get; set; } = false;
		public bool IsCurrent { get; set; } = true;
		public bool IsCancelled { get; set; } = false;
	}
}
