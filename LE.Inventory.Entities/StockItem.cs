using LE.Common.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Inventory.Entities
{
    public class StockItem
    {
        private string _name;

        [Key]
        public long stock_item_id { get; set; }

        [Required]
        [MaxLength(70)]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Name is required.");
                }
                _name = value;
            }
        }

        [Required]
        public long wood_type_id { get; set; }

        [Required]
        public long stock_unit_id { get; set; }

        public decimal threshold { get; set; }
   
        public decimal default_sales_rate { get; set; }
   
        [Required]
        public bool is_enabled { get; set; } = true;

        [ForeignKey("wood_type_id")]
        public virtual WoodType wood_type { get; set; }

        [ForeignKey("stock_unit_id")]
        public virtual StockUnit stock_unit { get; set; }

        public virtual StockItemAvailability stock_item_availability { get; set; }

        public virtual List<Purchase> purchases { get; set; }

        public bool hasPurchases()
        {
            return purchases.Count > 0;
        }

        public void enable()
        {
            is_enabled = true;
        }

        public void disable()
        {
            is_enabled = false;
        }
        
    }
}
