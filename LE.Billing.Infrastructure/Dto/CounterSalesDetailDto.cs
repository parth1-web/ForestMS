using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class CounterSalesDetailDto
    {
        public long sales_detail_id { get; set; }

        public long sales_id { get; set; }

        [Required(ErrorMessage = "Service Id is required.")]
        public long service_id { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Rate is required.")]
        public decimal rate { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Quantity is required.")]
        public decimal qty { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal discount { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal tax { get; set; }
    }
}
