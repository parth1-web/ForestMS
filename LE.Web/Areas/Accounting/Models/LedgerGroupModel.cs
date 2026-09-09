using LE.Account.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Accounting.Models
{
    public class LedgerGroupModel
    {
        public long ledger_group_id { get; set; }

        [Display(Name = "Group Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Group name  is required")]
        public string name { get; set; }

        [Display(Name = "Under Group")]
        public long parent_ledger_group_id { get; set; }

        [Display(Name = "Group Type")]

        public LedgerGroupType ledger_group_type { get; set; }

        public bool is_custom { get; set; }

        public string code { get; set; }


    }
}
