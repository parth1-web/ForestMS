using LE.Account.Infrastructure.Reports.Reporter.ValueObjects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class AccountingReportVm
    {
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal OpeningBalance { get; set; }
        public List<AccountingReportVo> LeftReport = new List<AccountingReportVo>();
        public List<AccountingReportVo> RightReport = new List<AccountingReportVo>();
        public string organization_name { get; set; }
        public string logo { get; set; }
        public string address { get; set; }
        public string phone_no { get; set; }
        public string fiscalYear { get; set; }
        public string title { get; set; }
    }

    public class AccountingReportDecoratorVm
    {
        public AccountingReportType ReportType { get; set; }
        public List<AccountingReportVo> Report { get; set; }
        public bool HideTopLevel { get; set; }
    }

    public enum AccountingReportType
    {
        Normal,
        Split,
        DrCrWise
    }
}