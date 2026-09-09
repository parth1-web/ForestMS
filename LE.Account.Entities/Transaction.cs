using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Entities
{
    public class Transaction
    {
        private decimal _amount = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long transaction_id { get; set; }

        [Required]
        public DateTime transaction_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
        public string nep_transaction_date { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
        public string nep_entry_date { get; set; }

        public string remarks { get; set; }

        [Required]
        public VoucherType voucher_type { get; set; }

        [Required]
        public long voucher_no { get; set; }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal amount
        {
            get => _amount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The amount provided is invalid.");
                _amount = value;
            }
        }

        public virtual List<TransactionDetail> TransactionDetails { get; set; }
    }
}
