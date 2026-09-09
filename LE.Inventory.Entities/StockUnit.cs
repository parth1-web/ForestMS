using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Inventory.Entities
{
    public class StockUnit
    {
        private string _name, _shortName;

        [Key]
        public long stock_unit_id { get; set; }

        [Required]
        [MaxLength(50)]
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
        [MaxLength(10)]
        public string short_name
        {
            get => _shortName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Short name is required.");
                }
                _shortName = value;
            }
        }


        public bool is_enabled { get; set; } = false;

        public virtual List<StockItem> stockItems { get; set; }

        public bool hasStockItems()
        {
            return stockItems.Count > 0;
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
