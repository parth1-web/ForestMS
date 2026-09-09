using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class StockUnitIndexViewModel
    {
        public List<StockUnitDetail> stock_units { get; set; }
    }

    public class StockUnitDetail
    {
        public long stock_unit_id { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public bool is_enabled { get; set; }

    }
}
