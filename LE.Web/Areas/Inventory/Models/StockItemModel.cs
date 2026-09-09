using LE.Inventory.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Web.Areas.Inventory.Models
{
    public class StockItemModel
    {
        public long stock_item_id { get; set; }
        [Display(Name ="Item Name")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Item Name cannot be empty.")]
        public string name { get; set; }

        [Display(Name = "Wood Type")]   
        [Required(AllowEmptyStrings = false, ErrorMessage = "Wood Type cannot be empty.")]
        public long wood_type_id { get; set; }


        [Display(Name = "Stock Unit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Stock Unit cannot be empty.")]
        public long stock_unit_id { get; set; }

       
        [Display(Name ="Default Sales Rate")]
        public decimal default_sales_rate { get; set; }
        
        [Display(Name ="Threshold")]
        public decimal threshold { get; set; }

        public bool is_enabled { get; set; }

        [ForeignKey("wood_type_id")]
        public virtual WoodType wood_type { get; set; }
    }
}
