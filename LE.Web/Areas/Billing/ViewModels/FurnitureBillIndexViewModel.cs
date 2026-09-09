using LE.Billing.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class FurnitureBillIndexViewModel
    {
        public DateTime sales_date { get; set; }

        public long furniture_sales_id { get; set; }

        public string nep_sales_date { get; set; }

        public decimal amount { get; set; }

        public string print_date { get; set; }

        public string print_time { get; set; }

        public string remarks { get; set; }

        public string customer { get; set; }

        public string address { get; set; }

        public string user { get; set; }

        public string numWords { get; set; }

        public bool is_cancelled { get; set; }

        public string tole_no { get; set; }

        public string logo { get; set; }

        public List<FurnitureBillDetail> furniture_bill_details { get; set; }

    }
    public class FurnitureBillDetail
    {
        public long furniture_sales_detail_id { get; set; }

        public long furniture_id { get; set; }

        public Furniture furniture { get; set; }

        public decimal rate { get; set; }

        public decimal quantity { get; set; }

        public decimal amount { get; set; }

    }
}
