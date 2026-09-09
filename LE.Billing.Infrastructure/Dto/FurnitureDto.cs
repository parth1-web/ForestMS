using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class FurnitureDto
    {
        public long furniture_id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Furniture name is required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Category id is required.")]
        public long furniture_category_id { get; set; }

        public bool is_enabled { get; set; } = true;
    }
}
