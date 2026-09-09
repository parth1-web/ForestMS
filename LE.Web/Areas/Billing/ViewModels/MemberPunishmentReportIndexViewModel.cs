using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;


namespace LE.Web.Areas.Billing.ViewModels
{
    public class MemberPunishmentReportIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public PunishmentStatus status { get; set; }

        public List<MemberPunishmentReportDetails> member_punishment_report = new List<MemberPunishmentReportDetails>();

    }

    public class MemberPunishmentReportDetails
    {
        public long MemberPunishmentId { get; set; }
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
