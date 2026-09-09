using DateConverter.Core.Service_Factory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class WoodBillReportIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();

        public bool is_cancelled { get; set; } = false;

        public List<WoodBillReportDetails> wood_bill_datas = new List<WoodBillReportDetails>();
    }

    public class WoodBillReportDetails
    {
        public long wood_bill_id { get; set; }

        public DateTime bill_date { get; set; }

        public decimal amount { get; set; }
        public decimal tax_amount { get; set; }

        public string remarks { get; set; }
        public string nep_bill_date { get; set; }

        public string name { get; set; }

        public string address { get; set; }

        public long user_id { get; set; }

        public bool is_cancelled { get; set; }
    }

}
