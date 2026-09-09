using LE.Inventory.Entities;
using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class StockItemIndexViewModel
    {
        public List<StockItemDetail> stock_items { get; set; }
    }
    public class StockItemDetail
    {
        public long stock_item_id { get; set; }
        public string name { get; set; }
        public long wood_type_id { get; set; }
        public long stock_unit_id { get; set; }
        public decimal default_sales_rate { get; set; }
        public decimal threshold { get; set; }
        public bool is_enabled { get; set; }
        public virtual StockItemAvailability stock_item_availability { get; set; }
        public virtual WoodType wood_type { get; set; }
        public virtual StockUnit stock_unit { get; set; }
    }
}
