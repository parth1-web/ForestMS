using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Account.Infrastructure.Dto
{
    public class TransactionDetailDto
    {
        private long _ledgerId,_refLedgerId;
        private decimal _debitAmount,_creditAmount;

        public DateTime transaction_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required(AllowEmptyStrings =false, ErrorMessage ="Ledger Id is required.")]
        public long ledger_id {
            get => _ledgerId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Ledger id is not valid.");
                _ledgerId = value;
            }
        }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Reference Ledger Id is required.")]
        public long ref_ledger_id {
            get => _refLedgerId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Reference Ledger Id is not valid.");
                _refLedgerId = value;
            }
        }

        public decimal debit_amount
        {
            get => _debitAmount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Debit Amount is not valid.");
                }
                _debitAmount = value;
            }
        }

        public decimal credit_amount
        {
            get => _creditAmount;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Credit Amount is not valid.");
                }
                _creditAmount = value;
            }
        }
    }
}
