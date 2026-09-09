using LE.Inventory.Common.Enums;
using LE.Inventory.Entities;
using LE.Web.Models;
using System;
using System.Collections.Generic;

namespace LE.Web.Areas.Inventory.ViewModels
{
    public class WoodDetailsIndexViewModel : PaginationFilter
    {
        public long stock_category_purpose_id { get; set; }
        public long piling_id { get; set; }
        public string piling_title { get; set; } = "N/A";
        public long stock_type_id { get; set; }
        public string sold_type { get; set; }
        public string search_by_golia_number { get; set; }
        public List<WoodsItemDetail> wood_details { get; set; }
    }
    public class WoodsItemDetail
    {
        public long wood_details_id { get; set; }

        public long piling_id { get; set; }

        public long stock_category_purpose_id { get; set; }

        public string year { get; set; }

        public long stock_type_id { get; set; }

        public long balla_balli_category_id { get; set; }

        public string goliya_number { get; set; }

        public string tuna_no { get; set; }

        public long wood_type_id { get; set; }

        public decimal circle_size { get; set; }

        public decimal length { get; set; }

        public Grade grade { get; set; }

        public DateTime created_date { get; set; }

        public bool is_sold { get; set; }
        //public bool is_transferred_to_chiran { get; set; }

        public long? sales_id { get; set; }
        public decimal fresh_total_size { get; set; }
        public virtual WoodType wood_type { get; set; }
        public virtual Piling piling { get; set; }
        public virtual StockCategoryPurpose category_purpose { get; set; }
        public virtual List<DamagedWoodDetail> DamagedWoodDetails { get; set; }


        public decimal getDamagedsize()
        {
            decimal result = 0;
            if (DamagedWoodDetails.Count > 0)
            {
                foreach (var detail in DamagedWoodDetails)
                {
                    result += detail.total_damaged_size;
                }
            }
            return result;
        }

        public decimal getNetTotal()
        {
            return Math.Round(fresh_total_size - getDamagedsize(), 2);

        }
    }
}
