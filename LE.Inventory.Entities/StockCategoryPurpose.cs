using LE.Common.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Entities
{
    public class StockCategoryPurpose
    {
        private string _name;

        [Key]
        public long stock_category_purpose_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Category Purpose Name cannot be empty.");
                }
                _name = value;
            }
        }

        [Required]
        public bool is_enabled { get; set; } = true;

        public virtual List<WoodDetails> wood_details { get; set; }

        public bool hasWoodDetails()
        {
            return wood_details.Count > 0;
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
