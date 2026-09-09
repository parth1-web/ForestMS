using DateConverter.Core.Service_Factory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.Models
{
    public class JournalModel
    {
        [Display(Name = "Date")]
        public string nep_transaction_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();

        public DateTime transaction_date { get; set; }

        [Display(Name = "Voucher No")]
        public long voucher_no { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        public List<JournalModelDetails> journalModelDetails { get; set; }

    }

    public class JournalModelDetails
    {
        //public long type { get; set; }
        public long ledger_id { get; set; }
        public decimal dr_amount { get; set; }
        public decimal cr_amount { get; set; }

    }
}
