using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Entities
{
    public class TransactionDetail
    {
        private long _ledgerId, _refLedgerId;
        private decimal _drAmount = 0, _crAmount = 0;

        [Key]
        public long transaction_detail_id { get; set; }
        [Required]
        public long transaction_id { get; set; }
        [Required]
        public long ledger_id
        {
            get => _ledgerId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The ledger provided is invalid.");
                _ledgerId = value;
            }
        }

        [Required]
        public long ref_ledger_id
        {
            get => _refLedgerId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The Reference Ledger provided is invalid.");
                _refLedgerId = value;
            }
        }

        [Required]
        public DateTime transaction_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal dr_amount
        {
            get => _drAmount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The Debit amount provided is invalid.");
                _drAmount = value;
            }
        }

        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal cr_amount
        {
            get => _crAmount;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The Credit amount provided is invalid.");
                _crAmount = value;
            }
        }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal balance { get; set; }

        public bool isValid()
        {
            if (ledger_id == ref_ledger_id)
                return false;
            else
                return true;
        }
        [ForeignKey("ledger_id")]
        public virtual Ledger ledger { get; set; }

        [ForeignKey("transaction_id")]
        public virtual Transaction transaction { get; set; }

        public string getRefLedgerName()
        {
            return ledger.name;
        }

        public int fiscal_year_id { get; set; }
    }
}
