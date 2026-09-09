using System;
using System.ComponentModel.DataAnnotations;
using DateConverter.Core.Service_Factory;

namespace LE.Inventory.Infrastructure.Dto
{
    public class StockItemAvailabilityDto
    {
     
        public long stock_item_availability_id { get; set; }

        [Required]
        public long stock_item_id { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal qty { get; set; }

        [Required]
        public DateTime last_updated_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required]
        [MaxLength(15)]
        public string nep_last_updated_date { get; set; }
    }
}
