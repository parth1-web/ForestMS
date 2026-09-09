using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Entities
{
    public class FurnitureCategory
    {
        [Key]
        public long furniture_category_id { get; set; }

        [MaxLength(50)]
        [Required]
        public string name { get; set; }

        public string description { get; set; }

        public bool is_enabled { get; set; } = true;

        public virtual List<Furniture> furnitures { get; set; }

        public void enable()
        {
            is_enabled = true;
            furnitures.ForEach(a => a.is_enabled = true);
        }

        public void disable()
        {
            is_enabled = false;
            furnitures.ForEach(a => a.is_enabled = false);
        }

        public bool hasFurnitures() => furnitures.Count > 0;
    }
}
