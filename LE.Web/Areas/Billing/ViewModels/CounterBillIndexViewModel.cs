using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class CounterBillIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public bool is_cancelled { get; set; } = false;

        public long user_id { get; set; }
        public virtual LE.Entities.User.User user { get; set; }

        public List<CounterBillDetail> counter_bill_details = new List<CounterBillDetail>();

    }
    public class CounterBillDetail
    {
        public DateTime sales_date { get; set; }

        public long sales_id { get; set; }

        public string nep_sales_date { get; set; }

        public decimal bill_amount { get; set; }
        public decimal discount_amount { get; set; }
        public decimal tax_amount { get; set; }
        public decimal net_total { get; set; }
        public decimal return_amount { get; set; }

        public virtual List<CounterSalesDetail> counter_sales_details { get; set; }

        public string remarks { get; set; }

        public string address { get; set; }

        public long user_id { get; set; }

        public bool is_cancelled { get; set; }
        public virtual LE.Entities.User.User user { get; set; }


    }
}
