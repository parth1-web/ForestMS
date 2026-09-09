using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class CounterSales
    {
        private decimal _billAmount, _discountAmount, _netTotal, _returnAmount, _taxAmount;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long sales_id { get; set; }

        public long user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual LE.Entities.User.User user { get; set; }

        public DateTime sales_date { get; set; }

        [Required]
        [MaxLength(15)]
        public string nep_sales_date { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal bill_amount
        {
            get => _billAmount;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Bill Amount is required.");
                }
                _billAmount = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal discount_amount
        {
            get => _discountAmount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Discount amount cannot be negative.");
                }
                _discountAmount = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal tax_amount
        {
            get => _taxAmount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Tax amount cannot be negative.");
                }
                _taxAmount = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Required]
        public decimal net_total
        {
            get => _netTotal;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Net total cannot be negative.");
                }
                _netTotal = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal return_amount
        {
            get => _returnAmount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Return amount cannot be negative.");
                }
                _returnAmount = value;
            }
        }

        [MaxLength(120)]
        public string remarks { get; set; }

        public bool is_cancelled { get; set; } = false;

        public long cancelled_by { get; set; }

        public DateTime cancelled_date { get; set; }

        public virtual List<CounterSalesDetail> counter_sales_details { get; set; }
    }
}
