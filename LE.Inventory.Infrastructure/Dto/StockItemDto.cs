using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class StockItemDto
    {
        private string _name;

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
        public bool is_enabled { get; set; } = false;
    }
}
