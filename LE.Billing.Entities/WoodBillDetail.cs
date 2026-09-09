using LE.Common.Exceptions;
using LE.Inventory.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class WoodBillDetail
    {
        decimal _rate, _amount;

        [Key]
        public long wood_bill_detail_id { get; set; }

        [Required]
        public long wood_bill_id { get; set; }

        [ForeignKey("wood_bill_id")]
        public virtual WoodBill woodBill { get; set; }

        public long wood_details_id { get; set; }

        [ForeignKey("wood_details_id")]
        public virtual WoodDetails woodDetails { get; set; }

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
    }
}
