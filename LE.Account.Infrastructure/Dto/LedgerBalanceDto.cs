using LE.Account.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Infrastructure.Dto
{
    public class LedgerBalanceDto
    {
        public long ledger_balance_id { get; set; }
        public long ledger_id { get; set; }
        public long ledger_group_id { get; set; }
        public decimal balance { get; set; }
        public DateTime updated_date { get; set; } = DateTime.Now.Date;
        [ForeignKey("ledger_id")]
        public virtual Ledger ledger { get; set; }
        [ForeignKey("ledger_group_id")]
        public virtual LedgerGroup ledger_group { get; set; }
    }
}
