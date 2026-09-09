using LE.Web.Models;

namespace LE.Web.Areas.Inventory.FilterModel
{
    public class StockItemFilter:PaginationFilter
    {
        public string name { get; set; }
    }
}
