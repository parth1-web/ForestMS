using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Inventory.Models
{
    public class PurchaseModel
    {
        private decimal _qty;
  
        public long purchase_id { get; set; }
        

        [Required]
        [Display(Name="Item")]
        public long stock_item_id { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        [Display(Name="Quantity")]
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
        public long user_id { get; set; }

        public DateTime purchase_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

    }
}
