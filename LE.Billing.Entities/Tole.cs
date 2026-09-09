using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Entities
{
    public class Tole
    {
        private string _toleNo;
        [Key]
        public long tole_id { get; set; }
        [Required]
        public string tole_no {
            get => _toleNo;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidValueException("Tole No is required.");
                }
                _toleNo = value;
            }
        }
    }
}
