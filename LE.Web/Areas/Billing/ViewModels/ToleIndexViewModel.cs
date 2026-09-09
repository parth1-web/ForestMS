using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class ToleIndexViewModel
    {
        public List<ToleDetail> toles { get; set; }

    }
    public class ToleDetail
    {
        public long tole_id { get; set; }
        public string tole_no { get; set; }
       
    }
}
