using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class TransactionIndexViewModel : GenericIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        [Display(Name = "To Date")]
        public string end_date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public long ledger_id { get; set; }
        public List<TransactionModel> transaction { get; set; }
    }

    public class TransactionModel
    {
        public DateTime transaction_date { get; set; }
        public string nep_transaction_date { get; set; }
        public DateTime entry_date { get; set; }
        public string nep_entry_date { get; set; }
        public string remarks { get; set; }
        public VoucherType voucher_type { get; set; }
        public long voucher_no { get; set; }
        public List<TransactionDetailModel> transactionDetail { get; set; }
    }

    public class TransactionDetailModel
    {
        public decimal dr_amount { get; set; }
        public decimal cr_amount { get; set; }
        public long ledger_id { get; set; }
        public long ref_ledger_id { get; set; }
        public DateTime transaction_date { get; set; }
        public decimal balance { get; set; }
        public string name { get; set; }
        public virtual Ledger ledger { get; set; }
        public virtual Transaction transaction { get; set; }
    }
}
