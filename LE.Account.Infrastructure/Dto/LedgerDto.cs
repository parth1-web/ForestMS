using LE.Account.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Account.Infrastructure.Dto
{
    public class LedgerDto
    {
        private string _name;
        private long _userId;
        private decimal _opening_balance;

        
        public long ledger_id { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Ledger name is required.")]
        [MinLength(3), MaxLength(30)]
        public string name {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new NonEmptyValueException("Ledger name must be provided.");
                _name = value;
            }
        }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Ledger Group id is required.")]
        public long ledger_group_id { get; set; }

        [Required]
        public string code { get; set; }

        [Required]
        public DateTime created_date { get; set; } = DateTime.Now;

        [Required(AllowEmptyStrings =false, ErrorMessage ="User Id is required.")]
        public long user_id {
            get => _userId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("User Id is not valid.");
                        _userId = value;
            }
        }
        public OpeningBalanceType balance_type { get; set; }
        public decimal opening_balance {
            get => _opening_balance;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Opening Balance cannot be less than zero.");
                _opening_balance = value;
            }
        }

    }
}
