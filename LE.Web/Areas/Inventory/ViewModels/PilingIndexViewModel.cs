using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class PilingIndexViewModel
    {
        public List<Pilings> pilings { get; set; }
    }

    public class Pilings
    {
        public long piling_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool is_enabled { get; set; }
    }
}
