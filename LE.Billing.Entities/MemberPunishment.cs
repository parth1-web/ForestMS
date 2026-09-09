using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LE.Billing.Entities
{
    public class MemberPunishment
    {
        [Key]
        public long MemberPunishmentId { get; set; }
        [Required]
        public long MembershipId { get; set; }
        [ForeignKey(nameof(MembershipId))]
        public virtual Membership Membership { get; set; }
        public string IllegalActivity { get; set; }
		public DateTime IssueDate { get; set; }
		public string NepIssueDate { get; set; }
		public DateTime PunishmentValidity { get; set; }
        public string NepPunishmentValidity { get; set; }
        public bool IsActive { get; set; } = true;
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long CreatedBy { get; set; }
        public bool IsCancelled { get; set; }
        public string IsCancelledRemarks { get; set; }
    }
}
