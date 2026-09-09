using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class CategoryPurposeIndexViewModel
    {
        public List<CategoryPurposeDetails> category_purposes { get; set; }
    }

    public class CategoryPurposeDetails
    {
        public long stock_category_purpose_id { get; set; }
        public string name { get; set; }
    }
}
