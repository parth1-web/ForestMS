using LE.Billing.Entities;
using LE.Common.Enums;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class MemberIndexViewModel
    {
        public List<MemberDetail> Members { get; set; }

    }
    public class MemberDetail
    {
        public long MemberId { get; set; }
        public long MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
		public string MemberCitizenship { get; set; }
		public string FullName { get; set; }
		public string Address { get; set; }
        public string ContactNo { get; set; }
		public int Age { get; set; }
		public GenderType Gender { get; set; }
		public bool IsGharmuli { get; set; }
		public bool IsDeleted { get; set; }
		public long CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string ImageName { get; set; }
		public RelationType FamilyRole { get; set; }
        public bool IsPunished { get; set; }
        public string NepPunishmentValidity { get; set; }
    }
}
