using DateConverter.Core.Service_Factory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Billing.Infrastructure.Dto
{
    public class ServiceDto
    {
        public long service_id { get; set; }

        [Required(ErrorMessage = "Category id is required.")]
        public long category_id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Service name is required.")]
        [MaxLength(100)]
        public string name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Rate is required.")]
        [Range(0, double.MaxValue)]
        public decimal rate { get; set; }

        public long created_by { get; set; }

        [Required(ErrorMessage = "Ledger Id is Required.")]
        public long ledger_id { get; set; }
        public decimal tax { get; set; }
        public DateTime created_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
        public bool is_enabled { get; set; } = true;
    }
}
