using LE.Inventory.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Inventory.Models
{
    public class WoodDetailsModel
    {
        public long wood_details_id { get; set; }

        [Display(Name = "प्रयोजन")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Purpose cannot be empty.")]
        public long stock_category_purpose_id { get; set; }

        [Display(Name = "पाईलिङ्")]
        public long piling_id { get; set; }

        [Display(Name = "वर्ष")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Year cannot be empty.")]
        public string year { get; set; }

        [Display(Name = "काठको प्रकार ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Stock Type cannot be empty.")]
        public long stock_type_id { get; set; }

        [Display(Name = "बल्लाबल्ली बर्ग ")]
        public long balla_balli_category_id { get; set; }

        [Display(Name = "काठको जात")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Wood Type cannot be empty.")]
        public long wood_type_id { get; set; }

        [Display(Name = "गोलाई इन्चमा")]
        public decimal circle_size { get; set; }

        [Display(Name = "लम्बई फिटमा")]
        public decimal length { get; set; }

        [Display(Name = "ग्रदिङ्")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Grade cannot be empty.")]
        public Grade grade { get; set; }

        [Display(Name = "घाटगद्धि गोलिया नं")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Goliya Number cannot be empty.")]
        public string goliya_number { get; set; }

        [Display(Name = "टुना नं")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tuna Number cannot be empty.")]
        public string tuna_no { get; set; }

        public List<DamagedWoodDetailModel> damagedWoodDetailModels { get; set; }
    }

    public class DamagedWoodDetailModel
    {
        public long damaged_wood_details_id { get; set; }
        public decimal damaged_first_size { get; set; }
        public decimal damaged_second_size { get; set; }
        public decimal damaged_third_size { get; set; }
        public decimal damaged_fourth_size { get; set; }
        public decimal damaged_fifth_size { get; set; }
        public long dividor_value { get; set; } = 2;
        public decimal damaged_feet_size { get; set; }
    }
}
