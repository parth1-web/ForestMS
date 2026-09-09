using LE.Billing.Entities;
using LE.Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Billing.Models
{
	public class MemberModel
	{
		public long MemberId { get; set; }

		public long MembershipId { get; set; }

		public virtual Membership Membership { get; set; }

		[DisplayName("Member Fullname")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Member name is required.")]
		public string FullName { get; set; }

		[DisplayName("Member Citizenship No.")]
		public string MemberCitizenship { get; set; }

		[DisplayName("Address")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Address is required.")]
		public string Address { get; set; }

		[DisplayName("Contact No.")]
		public string ContactNo { get; set; }

		[DisplayName("Age")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Age is required.")]
		public int Age { get; set; }

		[DisplayName("Gender")]
		public GenderType Gender { get; set; }

		[DisplayName("Is Gharmuli")]
		public bool IsGharmuli { get; set; }

		public bool IsActive { get; set; } = true;

		public bool IsDeleted { get; set; }

		[DisplayName("Member Image")]
		public IFormFile ImageName { get; set; }

		[DisplayName("Family Role")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Family role is required.")]
		public RelationType FamilyRole { get; set; }

		public long CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }
	}
}
