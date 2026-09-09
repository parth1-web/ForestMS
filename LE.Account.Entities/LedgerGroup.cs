using LE.Account.Common.Enums;
using LE.Common.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Account.Entities
{
    public class LedgerGroup
    {
        private string _name;

        [Key]
        public long ledger_group_id { get; set; }

        [Required]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new NonEmptyValueException("Ledger group name cannot be empty");
                _name = value;
            }
        }

        [Required]
        public string code { get; set; }

        [Required]
        public LedgerGroupType ledger_group_type { get; set; }

        [Required]
        public bool is_custom { get; set; } = true;

        public virtual List<Ledger> ledgers { get; set; }

        [Required]
        public long parent_ledger_group_id { get; set; }

        public long ledgers_count { get; set; }

        public virtual int getLedgersCount()
        {
            return ledgers == null ? 0 : ledgers.Count;
        }


    }

}
