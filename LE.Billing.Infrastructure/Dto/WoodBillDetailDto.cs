using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using LE.Inventory.Entities;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class WoodBillDetailDto
    {
        decimal _rate, _amount;

        public long wood_bill_detail_id { get; set; }

        [Required]
        public long wood_bill_id { get; set; }

        public long wood_details_id { get; set; }

        public long stock_type_id { get; set; }

        public virtual WoodDetails woodDetails{get; set;} 

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
    }
}
