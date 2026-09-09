using LE.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
	public class Member
	{
		[Key]
		public long MemberId { get; set; }
		public long MembershipId { get; set; }
		[ForeignKey(nameof(MembershipId))]
		public virtual Membership Membership { get; set; }
		[MaxLength(50)]
		public string MemberCitizenship { get; set; }
		[Required]
		[MaxLength(100)]
		public string FullName { get; set; }
		[MaxLength(150)]
		public string Address { get; set; }
		[MaxLength(20)]
		public string ContactNo { get; set; }
		public int Age { get; set; }
		public GenderType Gender { get; set; }
		public bool IsGharmuli { get; set; }
		public long CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public bool IsActive { get; set; }
		public bool IsDeleted{ get; set; }
		public string ImageName { get; set; }
		public RelationType FamilyRole { get; set; }
		public void Enable()
		{
			IsActive = true;
		}
		public void Disable()
		{
			IsActive = false;
		}
		public void Delete()
		{
			IsDeleted = true;
		}
	}
}
