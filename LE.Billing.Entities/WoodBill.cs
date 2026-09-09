using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class WoodBill
    {
        decimal _amount;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long wood_bill_id { get; set; }

        [Required]
        public DateTime bill_date { get; set; } 

        [Required]
        public string nep_bill_date { get; set; }
        [Required]
        public DateTime entry_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

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

        public long user_id { get; set; }

        public bool is_cancelled { get; set; } = false;

        public long cancelled_by { get; set; }

        public DateTime cancelled_date { get; set; }

        public virtual List<WoodBillDetail> wood_bill_detail { get; set; }

        public virtual List<WoodBillMemberTransaction> WoodBillMemberTransactions { get; set; } 
        public void cancel()
        {
            is_cancelled = true;
        }
    }
}
