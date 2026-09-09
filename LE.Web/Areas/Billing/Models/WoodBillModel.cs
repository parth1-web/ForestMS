using DateConverter.Core.Service_Factory;
using System.Collections.Generic;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.Models
{
    public class WoodBillModel
    {
        public string nep_bill_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public string sales_type { get; set; }
        public long? MemberId { get; set; }

        public string sales_type_id { get; set; }
        public string other_customer { get; set; }
        public string address { get; set; }
        public decimal total_amount { get; set; }
        public decimal tender_amount { get; set; }
        public decimal tax_percentage { get; set; }
        public string remarks { get; set; }
        public List<MemberList> members { get; set; }
        public List<Items> items { get; set; }
    }

    public class Items
    {
        public long wood_details_id { get; set; }
        public decimal rate { get; set; }
    }

    public class MemberList
    {
        public long MemberId { get; set; }
    }
}
