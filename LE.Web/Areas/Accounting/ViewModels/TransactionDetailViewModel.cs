using System.Collections.Generic;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class TransactionDetailViewModel
    {
        public long ledger_id { get; set; }
        public string name { get; set; }
        public decimal amount { get; set; }
    }
}
