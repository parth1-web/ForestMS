using LE.Billing.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class FurnitureIndexViewModel
    {
        public List<FurnitureDetail> furnitures { get; set; }

    }
    public class FurnitureDetail
    {
        public long furniture_id { get; set; }
        public virtual FurnitureCategory furniture_category { get; set; }
        public string name { get; set; }
        public DateTime created_date { get; set; }
        public bool is_enabled { get; set; }
    }
}
