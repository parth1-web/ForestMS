using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DateConverter.Core.Service_Factory;
namespace LE.Billing.Entities
{
    public class ChiranSales
    {
        decimal _amount;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long chiran_sales_id { get; set; }

        [Required]
        public DateTime sales_date { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required]
        public string nep_sales_date { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal amount
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

        public string name { get; set; }

        public decimal tax_amount { get; set; }

        public string address { get; set; }

        [Required]
        public SalesType sales_type { get; set; }

        public long? member_id { get; set; }

        public long user_id { get; set; }

        public bool is_cancelled { get; set; } = false;

        public DateTime cancelled_date { get; set; }

        public long cancelled_by { get; set; }

        public virtual List<ChiranSalesDetail> chiran_sales_detail { get; set; }

        public void cancel()
        {
            is_cancelled = true;
        }
    }
}
