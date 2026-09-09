using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.Models
{
    public class MemberPunishmentModel
    {
        public long MemberPunishmentId { get; set; }
        [DisplayName("Member")]
        [Required]
        public long MembershipId { get; set; }
        //[DisplayName("Member")]
        //[Required]
        //public long MemberId { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public string IsCancelledRemarks { get; set; }
        public bool IsCancelled { get; set; }
        public virtual List<MemberModel> MemberDetails { get; set; }

    }
}
