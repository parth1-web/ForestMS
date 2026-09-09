using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class WoodTypeIndexViewModel
    {
        public List<Woods> wood_types { get; set; }
    }

    public class Woods
    {
        public long wood_type_id { get; set; }
        public string name { get; set; }
    }
}
