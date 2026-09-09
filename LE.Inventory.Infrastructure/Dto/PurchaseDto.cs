using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class PurchaseDto
    {
        private decimal _qty;

        [Key]
        public long purchase_id { get; set; }

        [Required]
        public long user_id { get; set; }


        [Required]
        public long stock_item_id { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal qty
        {
            get => _qty;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Quantity is invalid.");
                }
                _qty = value;
            }
        }

        [Required]
        public DateTime purchase_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
    }
}
