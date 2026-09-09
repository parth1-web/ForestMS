using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Entities
{
    public class Receipt
    {
        private long _receiptTo, _receiptFrom, _userId;
        private decimal _amount = 0, _discount = 0;

        [Key]
        [Required]
        public long receipt_id { get; set; }

        [Required]
        public long receipt_to
        {
            get => _receiptTo;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("The receipt To value provided is invalid.");
                _receiptTo = value;

            }
        }

        [Required]
        public long receipt_from
        {
            get => _receiptFrom;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("The receipt from value provided is invalid.");
                _receiptFrom = value;
            }
        }
        [Required]
        public DateTime transaction_date { get; set; }
        public string nep_transaction_date { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
        public string nep_entry_date { get; set; }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal amount
        {
            get => _amount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The value provided is invalid.");
                _amount = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal discount
        {
            get => _discount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The value provided is invalid.");
                _discount = value;
            }
        }

        public string remarks { get; set; }

        [Required]
        public long user_id
        {
            get => _userId;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("The user provided is invalid.");
                _userId = value;
            }
        }

        public string cheque_date { get; set; }
        public string cheque_no { get; set; }
        public string customer_name { get; set; }

        public bool is_cancelled { get; set; } = false;
        public long cancelled_by { get; set; }

        public DateTime cancelled_date { get; set; }

    }
}
