using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class ReceiptDto
    {
        private long _receiptTo, _receiptFrom, _userId;
        private decimal _amount, _discount;

        public long receipt_id { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Receipt To id is required")]
        public long receipt_to {
            get => _receiptTo;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("Receipt To ID is invalid.");
                _receiptTo = value;
            }
        }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Receipt From ID is required")]
        public long receipt_from {
            get => _receiptFrom;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("Receipt From Id is required.");
                    _receiptFrom = value;
            }
        }

        public DateTime transaction_date { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Amount is required.")]
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
        public string customer_name { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="User ID is required.")]
        public long user_id {
            get => _userId;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("User Id is not valid.");
                _userId = value;
            }
        }
        public bool isValid()
        {
            if (amount <= 0 || receipt_from <= 0 || receipt_to <= 0 || discount < 0)
                return false;
            else
                return true;
        }
    }
}
