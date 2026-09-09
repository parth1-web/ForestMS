using LE.Billing.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class MemberPunishmentViewModel
    {
        public List<MemberPunishmentDetail> MemPunishment { get; set; }
    }

    public class MemberPunishmentDetail
    {
        public long MemberPunishmentId { get; set; }
        public long MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        public string IllegalActivity { get; set; }
        public DateTime IssueDate { get; set; }
        public string NepIssueDate { get; set; }
        public DateTime PunishmentValidity { get; set; }
        public string NepPunishmentValidity { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public bool IsPunished { get; set; }
        public string IsCancelledRemarks { get; set; }
    }
}
