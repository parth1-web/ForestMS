using LE.Common.Exceptions;
using LE.Inventory.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class ChiranSalesDetail
    {
        decimal _rate, _amount, _circleSize, _length, _breadth, _quantity, _totalSize;

        [Key]
        public long chiran_sales_detail_id { get; set; }

        public long chiran_sales_id { get; set; }

        [ForeignKey("chiran_sales_id")]
        public virtual ChiranSales chiranSales { get; set; }

        [Required]
        public long wood_type_id { get; set; }

        [ForeignKey("wood_type_id")]
        public virtual WoodType woodType { get; set; }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal circle_size
        {
            get => _circleSize;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Circle Size cannot be zero or negative.");
                }
                _circleSize = value;
            }
        }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal breadth
        {
            get => _breadth;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Breadth of wood cannot be zero or negative");
                }
                _breadth = value;
            }
        }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal length
        {
            get => _length;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Length of wood cannot be zero or negative.");
                }
                _length = value;
            }
        }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal quantity
        {
            get => _quantity;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Quantity cannot be negative or zero.");
                }
                _quantity = value;
            }
        }

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

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal total_size
        {
            get => _totalSize;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Total Size cannot be Negative.");
                }
                _totalSize = value;
            }
        }
    }
}
