using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Billing.Models
{
    public class FurnitureCategoryModel
    {
        public long furniture_category_id { get; set; }

        [Display(Name = "Category Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Name is required")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }
    }
}
