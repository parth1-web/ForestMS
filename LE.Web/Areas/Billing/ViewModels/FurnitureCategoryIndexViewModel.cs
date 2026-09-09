using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class FurnitureCategoryIndexViewModel
    {
        public List<FurnitureCategoryDetail> furniture_categories { get; set; }

    }
    public class FurnitureCategoryDetail
    {
        public long furniture_category_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool is_enabled { get; set; }
    }
}
