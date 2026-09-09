using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class FurnitureSalesDetailDto
    {
        public long furniture_sales_detail_id { get; set; }

        public long furniture_sales_id { get; set; }

        public long furniture_id { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal rate { get; set; }


        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal quantity { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal amount { get; set; }
    }
}
