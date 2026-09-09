using LE.Account.Common.Enums;
using LE.Account.Entities;
using System.Collections.Generic;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class LedgerGroupIndexViewModel
    {
        public long ledger_group_id { get; set; }
        public string group_name { get; set; }
        public string all { get; set; }
        public List<LedgerGroupDetailModel> ledger_group_details { get; set; }

    }

    public class LedgerGroupDetailModel
    {
        public long ledger_group_id { get; set; }
        public string name { get; set; }
        public LedgerGroupType ledger_group_type { get; set; }
        public bool is_custom { get; set; }
        public long parent_ledger_group_id { get; set; }
        public string parent_group_name { get; set; }
        public virtual List<Ledger> ledgers { get; set; }

        public virtual long getLedgersCount()
        {
            return ledgers == null ? 0 : ledgers.Count;
        }

    }
}
