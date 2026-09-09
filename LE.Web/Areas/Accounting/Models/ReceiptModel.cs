using DateConverter.Core.Service_Factory;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.Models
{
    public class ReceiptModel
    {
        public long receipt_id { get; set; }

        [Display(Name = "Receipt Mode")]
        public long receipt_to { get; set; }

        [Display(Name = "Receipt From")]
        public long receipt_from { get; set; }

        [Display(Name = "Amount")]
        public decimal amount { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Cheque No")]
        public string cheque_no { get; set; }

        [Display(Name = "Cheque Date")]
        public string cheque_date { get; set; }

        [Display(Name = "Receipt Date")]
        public string nep_transaction_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();

        [Display(Name = "Person Name")]
        public string customer_name { get; set; }

    }
}
