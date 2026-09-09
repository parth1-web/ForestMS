using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class FurnitureSalesDetail
    {
        decimal _rate, _amount, _qty;

        [Key]
        public long furniture_sales_detail_id { get; set; }

        [Required]
        public long furniture_sales_id { get; set; }

        [ForeignKey(nameof(furniture_sales_id))]
        public virtual FurnitureSales furnitureSales { get; set; }

        [Required]
        public long furniture_id { get; set; }

        [ForeignKey(nameof(furniture_id))]
        public virtual Furniture furniture { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal rate
        {
            get => _rate;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Rate cannot be less than zero.");
                }
                _rate = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal quantity
        {
            get => _qty;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Quantity cannot be less than equal to zero.");
                }
                _qty = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal amount
        {
            get => _amount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Amount cannot be Negative.");
                }
                _amount = value;
            }
        }
    }
}
