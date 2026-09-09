using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class ChiranSalesDetailDto
    {
        decimal _rate, _amount, _circleSize, _length, _breadth, _quantity,_totalSize;

        public long chiran_sales_detail_id { get; set; }

        [Required]
        public long chiran_sales_id { get; set; }

        public long wood_type_id { get; set; }

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
        public decimal rate
        {
            get => _rate;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Rate cannot be less than or equal to zero.");
                }
                _rate = value;
            }
        }
       
        [RegularExpression(@"^\d+\.\d{0,2}$")]
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

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal total_size
        {
            get => _totalSize;
            set
            {
                if (value <=0)
                {
                    throw new InvalidValueException("Total Size cannot be Negative.");
                }
                _totalSize = value;
            }
        }
    }
}
