using DateConverter.Core.Service_Factory;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class ReceiptIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public bool is_cancelled { get; set; } = false;
        public List<ReceiptDetails> receipts { get; set; }
    }

    public class ReceiptDetails
    {
        public long receipt_id { get; set; }
        public string receipt_to_name { get; set; }
        public string receipt_from_name { get; set; }
        public string nep_transaction_date { get; set; }
        public string cheque_no { get; set; }
        public string cheque_date { get; set; }
        public decimal amount { get; set; }
        public decimal discount { get; set; }
        public string remarks { get; set; }
        public string customer_name { get; set; }
        public long user_id { get; set; }
        public bool is_cancelled { get; set; }
    }
}
