using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class PaymentDto
    {
        private decimal _amount,_discount;
        private long _userId;

        public long payment_id { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Payment To id is required.")]
        public long payment_to { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Payment From Id is required.")]
        public long payment_from { get; set; }

        [Required]
        public DateTime transaction_date { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="Amount is required.")]
        public decimal amount {
            get => _amount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Amount is not valid.");
                _amount = value;
            }
        }
        
        public decimal discount {
            get => _discount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Discount is not valid.");
                _discount = value;
            }
        }

        public string remarks { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="User id is required.")]
        public long user_id {
            get => _userId;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("User id is not valid.");
                _userId = value;
            }
        }

        public bool isValid()
        {
            if (amount <= 0 || payment_from <= 0 || payment_to <= 0 || discount < 0)
                return false;
            else
                return true;
        }
    }
}
