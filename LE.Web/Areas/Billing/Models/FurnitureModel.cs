using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Billing.Models
{
    public class FurnitureModel
    {
        public long furniture_id { get; set; }

        [Display(Name = "Furniture Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Furniture Title is required")]
        public string name { get; set; }

        [Display(Name = "Category")]
        public long furniture_category_id { get; set; }

        [Display(Name = "is_enabled")]
        public bool is_enabled { get; set; } = true;
    }
}
