using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Inventory.Models
{
    public class CategoryPurposeModel
    {
        public long stock_category_purpose_id { get; set; }

        [Display(Name ="Purpose Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Purpose Name is required")]
        public string name { get; set; }
    }
}
