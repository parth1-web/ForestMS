using LE.Inventory.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class WoodBillIndexViewModel
    {
        public DateTime bill_date { get; set; }

        public long wood_bill_id { get; set; }

        public string nep_bill_date { get; set; }

        public decimal amount { get; set; }
        public decimal tax_amount { get; set; }

        public string remarks { get; set; }

        public string print_date { get; set; }

        public string print_time { get; set; } = DateTime.Now.ToShortTimeString();

        public string user { get; set; }

        public bool is_cancelled { get; set; }

        public string numWords { get; set; }

        public string logo { get; set; }

        public List<string> members { get; set; }

        public List<string> address { get; set; }
        public List<string> tole_no { get; set; }

        public List<BillDetail> bill_details { get; set; }
    }
    public class BillDetail
    {
        public long wood_bill_detail_id { get; set; }

        public long wood_details_id { get; set; }

        public WoodDetails woodDetails { get; set; }

        public decimal rate { get; set; }

        public decimal amount { get; set; }
    }
}
