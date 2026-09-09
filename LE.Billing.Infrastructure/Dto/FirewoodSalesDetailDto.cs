using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class FirewoodSalesDetailDto
    {
        public long firewood_sales_detail_id { get; set; }

        public long firewood_sales_id { get; set; }

        public long stock_item_id { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal rate { get; set; }
        

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal quantity { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal amount { get; set; }
    }
}
