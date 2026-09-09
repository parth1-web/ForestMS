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
    public class FireWoodBillReportIndexViewModel
    {

        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public bool is_cancelled { get; set; } = false;

        public List<FirewoodBillReportDetails> firewood_bill_datas = new List<FirewoodBillReportDetails>();
    }

    public class FirewoodBillReportDetails
    {
        public DateTime sales_date { get; set; }

        public long firewood_sales_id { get; set; }

        public string nep_sales_date { get; set; }

        public FirewoodSalesType sales_type { get; set; }

        public decimal total_amount { get; set; }

        public string remarks { get; set; }

        [ForeignKey("type_id")]
        public virtual Member member { get; set; }
        public long? type_id { get; set; }

        public string others_name { get; set; }

        public string address { get; set; }

        public string user_id { get; set; }

        public bool is_cancelled { get; set; }
    }

}
