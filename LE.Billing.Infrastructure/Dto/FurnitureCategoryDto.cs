using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class FurnitureCategoryDto
    {
        public long furniture_category_id { get; set; }

        [MaxLength(100)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category name is required.")]
        public string name { get; set; }

        public string description { get; set; }

        public bool is_enabled { get; set; } = true;
    }
}
