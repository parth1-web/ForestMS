using LE.Account.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Accounting.Models
{
    public class LedgerModel
    {
        public long ledger_id { get; set; }

        [Display(Name = "Ledger Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ledger name  is required")]
        public string name { get; set; }

        [Display(Name = "Account Group")]
        public long ledger_group_id { get; set; }

        [Display(Name = "Balance Type")]
        public OpeningBalanceType balance_type { get; set; }

        [Display(Name = "Opening Balance")]
        public decimal opening_balance { get; set; }
        public long user_id { get; set; }

        [Display(Name = "Ledger Code")]
        public string code { set; get; }
    }
}
