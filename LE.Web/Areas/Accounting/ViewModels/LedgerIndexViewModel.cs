using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class ListLedgerIndexViewModel : GenericIndexViewModel
    {
        public string all { get; set; }
        public long ledger_group_id { get; set; }

        [Display(Name = "From Date")]
        public string start_date { get; set; }

        [Display(Name = "To Date")]
        public string end_date { get; set; }

        public List<LedgerIndexViewModel> listLedgerindexVM { get; set; }
    }

    public class LedgerIndexViewModel
    {
        public string group_name { get; set; }

        public List<LedgerDetailModel> ledger_details { get; set; }
    }
    public class LedgerDetailModel
    {
        public long ledger_id { get; set; }
        public string name { get; set; }
        public string nep_created_date { get; set; }
        public long ledger_group_id { get; set; }
        public string code { get; set; }
        public DateTime created_date { get; set; }
        public LedgerGroup ledger_group { get; set; }
        public decimal balance { get; set; }
    }
}
