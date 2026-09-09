using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Entities
{
    public class LedgerBalance
    {
        private long _ledgerId;
        private long _ledgerGroupId;

        [Key]
        public long ledger_balance_id { get; set; }

        public long ledger_id
        {
            get => _ledgerId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The ledger you provided is invalid.");
                _ledgerId = value;
            }
        }

        [ForeignKey("ledger_id")]
        public virtual Ledger ledger { get; set; }

        public long ledger_group_id
        {
            get => _ledgerGroupId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The ledger group you provided is invalid.");
                _ledgerGroupId = value;
            }
        }

        [ForeignKey("ledger_group_id")]
        public virtual LedgerGroup ledger_group { get; set; }

        public decimal balance { get; set; } = 0;

        public DateTime updated_date { get; set; } = DateTime.Now.Date;
    }
}
