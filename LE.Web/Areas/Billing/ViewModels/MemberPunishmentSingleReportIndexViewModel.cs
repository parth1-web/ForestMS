using LE.Billing.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using LE.Common.Enums;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class MemberPunishmentSingleReportIndexViewModel
    {
        public string logo { get; set; }
        public string print_date { get; set; }
        public string print_time { get; set; } = DateTime.Now.ToShortTimeString();
        public string user { get; set; }
        public MemberPunishment MemPunishmentReportDetail { get; set; }
    }
}
