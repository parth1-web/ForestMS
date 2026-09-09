using System;

namespace LE.Billing.Infrastructure.Dto
{
    public class MembershipValidityDto
    {
        public long MembershipValidityId { get; set; }
        public long MembershipId { get; set; }
        public virtual MembershipDto MembershipDto { get; set; }
        public string IssueDate { get; set; }
        public string NepValidityDate { get; set; }
        public DateTime ValidityDate { get; set; }
		public string RenewedDate { get; set; }
		public bool IsExpired { get; set; }
		public bool IsCurrent { get; set; }
		public bool IsCancelled { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long CreatedBy { get; set; }
    }
}
