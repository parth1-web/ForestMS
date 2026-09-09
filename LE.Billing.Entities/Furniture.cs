using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class Furniture
    {
        private long _furniture_category_id;

        [Key]
        public long furniture_id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public long furniture_category_id
        {
            get => _furniture_category_id;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Category id is invalid.");
                }
                _furniture_category_id = value;
            }
        }

        [ForeignKey("furniture_category_id")]
        public virtual FurnitureCategory furnitureCategory { get; set; }

        [Required]
        public bool is_enabled { get; set; } = true;

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
