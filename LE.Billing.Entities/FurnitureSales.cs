using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class FurnitureSales
    {
        decimal _amount;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long furniture_sales_id { get; set; }

        [Required]
        public DateTime sales_date { get; set; }

        [Required]
        public string nep_sales_date { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        public FirewoodSalesType sales_type { get; set; }

        public long? type_id { get; set; }

        public string others_name { get; set; }

        public string address { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal total_amount
        {
            get => _amount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Amount cannot be Negative.");
                }
                _amount = value;
            }
        }

        [MaxLength(150)]
        public string remarks { get; set; }

        public long user_id { get; set; }

        public bool is_cancelled { get; set; } = false;

        public DateTime cancelled_date { get; set; }

        public long cancelled_by { get; set; }

        public virtual List<FurnitureSalesDetail> furniture_sales_detail { get; set; }

        public void cancel()
        {
            is_cancelled = true;
        }
    }
}
