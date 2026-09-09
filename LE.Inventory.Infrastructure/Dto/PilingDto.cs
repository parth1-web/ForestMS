using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class PilingDto
    {
        [Key]
        public long piling_id { get; set; }

        [Required]
        public string title { get; set; }

        public string description { get; set; }

        public bool is_enabled { get; set; } = true;
    }
}
