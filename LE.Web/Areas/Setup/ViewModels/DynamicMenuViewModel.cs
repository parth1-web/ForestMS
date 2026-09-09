using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Setup.ViewModels
{
    public class DynamicMenuViewModel
    {
        public long dynamic_menu_id { get; set; }
        public string module_name { get; set; }
        public string parent_menu_name { get; set; }
        public string menu_name { get; set; }
        public string icon { get; set; }
        public string web_url { get; set; }
        public string api_url { get; set; }
        public int display_order { get; set; }
    }
}
