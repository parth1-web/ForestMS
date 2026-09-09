using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Account.Entities
{
    public class LedgerSetup
    {
        private string _key, _value;

        [Key]
        public long ledger_setup_id { get; set; }
        [Required]
        [MaxLength(70)]
        public string key
        {
            get => _key;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Key is required.");
                }
                _key = value;
            }
        }

        [Required]
        [MaxLength(500)]
        public string value
        {
            get => _value;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Value is required.");
                }
                _value = value;
            }
        }

    }
}
