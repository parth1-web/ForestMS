using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Inventory.Models
{
    public class WoodTypeModel
    {
        public long wood_type_id { get; set; }

        [Display(Name ="Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Wood Type Name is required")]
        public string name { get; set; }
    }
}
