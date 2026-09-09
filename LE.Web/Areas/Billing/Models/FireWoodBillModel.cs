using DateConverter.Core.Service_Factory;
using System.Collections.Generic;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.Models
{
    public class FireWoodBillModel
    {
        public string nep_sales_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public string sales_type { get; set; }
        public long? MemberId { get; set; }

        public long? type_id { get; set; }
        public string others_name { get; set; }
        public string address { get; set; }
        public decimal total_amount { get; set; }
        public decimal tender_amount { get; set; }
        public string remarks { get; set; }
        public decimal tax_percentage { get; set; }
        public List<firewoodItems> items { get; set; }
    }

    public class firewoodItems
    {
        public long stock_item_id { get; set; }
        public decimal rate { get; set; }
        public decimal quantity { get; set; }
    }
}
