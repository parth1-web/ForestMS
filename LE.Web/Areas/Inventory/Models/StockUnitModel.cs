using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Inventory.Models
{
    public class StockUnitModel
    {
        public long stock_unit_id { get; set; }

        [Display(Name="Unit Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unit Name is required")]
        public string name { get; set; }

        [Display(Name ="Short Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Short Name is required")]
        public string short_name { get; set; }
       

    }
}
