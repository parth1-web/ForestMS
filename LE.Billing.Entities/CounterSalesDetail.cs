using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class CounterSalesDetail
    {
        private decimal _rate, _qty, _taxAmount;

        [Key]
        public long sales_detail_id { get; set; }

        [Required]
        public long sales_id { get; set; }

        [ForeignKey("sales_id")]
        public virtual CounterSales sales { get; set; }

        [Required]
        public long service_id { get; set; }

        [ForeignKey("service_id")]
        public virtual Service service { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal rate
        {
            get => _rate;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Rate cannot be less than or equal to zero.");
                }
                _rate = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal tax_amount
        {
            get => _taxAmount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Tax Amount cannot be negative.");
                }
                _taxAmount = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal qty
        {
            get => _qty;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Qty cannot be less than or equal to zero.");
                }
                _qty = value;
            }
        }
    }
}
