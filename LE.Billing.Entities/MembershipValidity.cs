using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class MembershipValidity
    {
        [Key]
        public long MembershipValidityId { get; set; }
        [Required]
        public long MembershipId { get; set; }
        [ForeignKey(nameof(MembershipId))]
        public virtual Membership Membership { get; set; }
		public string IssueDate { get; set; }
		public string NepValidityDate { get; set; }
		public DateTime ValidityDate { get; set; }
        public string RenewedDate { get; set; }
		public bool IsExpired { get; set; }
		public bool IsCurrent{ get; set; }
		public bool IsCancelled { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long CreatedBy { get; set; }
        public void Expire()
        {
            IsExpired = true;
        }
        public void Unexpire()
        {
			IsExpired = false;
        }
    }
}
