using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Entities
{
    public class Payment
    {
        private long _paymentTo, _paymentFrom, _userId;
        private decimal _amount = 0, _discount = 0;

        [Key]
        public long payment_id { get; set; }

        [Required]
        public long payment_to
        {
            get => _paymentTo;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("The Id you provided is invalid.");
                _paymentTo = value;
            }
        }

        [Required]
        public long payment_from
        {
            get => _paymentFrom;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("The Id you provided is invalid.");
                _paymentFrom = value;
            }
        }

        [Required]
        public DateTime transaction_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
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
                    throw new InvalidValueException("The amount provided is invalid.");
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
                    throw new InvalidValueException("The discount provided is invalid.");
                _discount = value;
            }
        }

        [Required]
        public string remarks { get; set; }

        [Required]
        public long user_id
        {
            get => _userId;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("User Id is invalid");
                _userId = value;
            }
        }

        public string cheque_no { get; set; }
        public string cheque_date { get; set; }

        public bool is_cancelled { get; set; } = false;
        public long cancelled_by { get; set; }

        public DateTime cancelled_date { get; set; }
    }
}
