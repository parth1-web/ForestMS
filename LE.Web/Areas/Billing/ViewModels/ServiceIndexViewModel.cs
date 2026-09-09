using LE.Account.Entities;
using LE.Billing.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class ServiceIndexViewModel
    {
        public List<ServiceDetail> services { get; set; }
    }
    public class ServiceDetail
    {
        public long service_id { get; set; }
        public virtual ServiceCategory service_category { get; set; }
        public string name { get; set; }
        public decimal rate { get; set; }
        public DateTime created_date { get; set; }
        public long created_by { get; set; }
        public long ledger_id { get; set; }
        public virtual Ledger ledger { get; set; }
        public bool is_enabled { get; set; }
        public decimal tax { get; set; }
    }
}
