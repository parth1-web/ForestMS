using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Entities
{
    public class Piling
    {
        [Key]
        public long piling_id { get; set; }

        [Required]
        public string title { get; set; }

        public string description { get; set; }

        public bool is_enabled { get; set; } = true;

        public virtual List<WoodDetails> woodDetails { get; set; }

        public bool hasWoods()
        {
            return woodDetails.Count > 0;
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
