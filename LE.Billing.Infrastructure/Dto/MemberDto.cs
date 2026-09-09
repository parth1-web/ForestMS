using LE.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class MemberDto
    {
        public long MemberId { get; set; }
        public long MembershipId { get; set; }
		public string MemberCitizenship { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Full Name is required.")]
		public string FullName { get; set; }
        [MaxLength(150)]
        public string Address { get; set; }
        [MaxLength(20)]
		public string ContactNo { get; set; }
		public int Age { get; set; }
		public GenderType Gender { get; set; }
        public bool IsGharmuli { get; set; } = false;
        public long CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted{ get; set; } = false;
        public string ImageName { get; set; }
		public RelationType FamilyRole { get; set; }
    }
}
