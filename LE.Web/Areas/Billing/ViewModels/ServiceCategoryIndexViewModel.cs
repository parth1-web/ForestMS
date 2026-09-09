using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class ServiceCategoryIndexViewModel
    {
        public List<ServiceCategoryDetail> service_categories { get; set; }

    }
    public class ServiceCategoryDetail
    {
        public long category_id { get; set; }
        public string name { get; set; }
        public bool is_enabled { get; set; }
    }
}
