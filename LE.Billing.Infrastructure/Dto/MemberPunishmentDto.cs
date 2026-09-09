using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class MemberPunishmentDto
    {
        public long MemberPunishmentId { get; set; }
        [DisplayName("Membership")]
        [Required]
        public long MembershipId { get; set; }
        [DisplayName("Illegal Activity")]
        public string IllegalActivity { get; set; }
		public DateTime IssueDate { get; set; } = DateTime.Now;
		public string NepIssueDate { get; set; }
		public DateTime PunishmentValidity { get; set; }
        [DisplayName("Punishment Validity")]
        public string NepPunishmentValidity { get; set; }
		public bool IsActive { get; set; }
		[DisplayName("Remarks")]
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long CreatedBy { get; set; }
        public bool IsCancelled { get; set; }

        [DisplayName("CancelRemarks")]
        //[Required]
        public string IsCancelledRemarks { get; set; }
        public virtual List<Member> MemberDetails { get; set; }


    }
}
