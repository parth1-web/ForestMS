using LE.Inventory.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class ChiranBillIndexViewModel
    {
        public DateTime sales_date { get; set; }

        public long chiran_sales_id { get; set; }

        public string nep_sales_date { get; set; }

        public decimal amount { get; set; }

        public decimal tax_amount { get; set; }

        public string remarks { get; set; }

        public string print_date { get; set; }

        public string print_time { get; set; } = DateTime.Now.ToShortTimeString();

        public string user { get; set; }

        public bool is_cancelled { get; set; }

        public string numWords { get; set; }

        public string logo { get; set; }

        public string address { get; set; }

        public string member { get; set; }

        public string tole_no { get; set; }

        public List<ChiranDetail> bill_details { get; set; }
    }
    public class ChiranDetail
    {
        public long wood_type_id { get; set; }
        public WoodType wood_type { get; set; }
        public decimal rate { get; set; }
        public decimal circle_size { get; set; }
        public decimal length { get; set; }
        public decimal breadth { get; set; }
        public decimal quantity { get; set; }
    }
}
