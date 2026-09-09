using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using System;
using System.Collections.Generic;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.Models
{
    public class ChiranBillModel
    {
        public string nep_sales_date { get; set; }= DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public SalesType sales_type { get; set; }
        public long? MemberId { get; set; }
        public string other_customer { get; set; }
        public string address { get; set; }
        public decimal total_amount { get; set; }
        public decimal tender_amount { get; set; }
        public decimal tax_percentage { get; set; }
        public string remarks { get; set; }

        public List<ChiranItems> chiranItems { get; set; }

    }

    public class ChiranItems
    {
        public long wood_type_id { get; set; }
        public decimal rate { get; set; }
        public decimal circle_size { get; set; }
        public decimal length { get; set; }
        public decimal breadth { get; set; }
        public decimal quantity { get; set; }
    }

}
