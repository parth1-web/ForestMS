using LE.Inventory.Entities;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class PurchaseIndexViewModel
    {
        public List<PurchaseDetail> purchase_items { get; set; }
    }
    public class PurchaseDetail
    {
        public long purchase_id { get; set; }
        public decimal qty { get; set; }
        public DateTime purchase_date { get; set; }
        public string nep_purchase_date { get; set; }
        public long stock_item_id { get; set; }
        public long user_id { get; set; }

        public virtual StockItem stock_items { get; set; }
    }
}
