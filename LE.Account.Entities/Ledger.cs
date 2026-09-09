using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Account.Entities
{
    public class Ledger
    {
        private string _name;
        private long _userId;
        private long _ledgerGroupId;

        [Key]
        public long ledger_id { get; set; }

        [Required]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new NonEmptyValueException("Ledger name cannot be empty.");
                _name = value;
            }
        }

        [Required]
        public long ledger_group_id
        {
            get => _ledgerGroupId;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("The ledger group you provided is invalid.");
                _ledgerGroupId = value;
            }
        }
        [Required]
        public string code { get; set; }
        public DateTime created_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
        public string nep_created_date { get; set; }

        [Required]
        public long user_id
        {
            get => _userId;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("The User Id you provided is not valid.");
                _userId = value;

            }
        }

        public bool is_currently_used { get; set; } = false;

        [ForeignKey("ledger_group_id")]
        public virtual LedgerGroup ledger_group { get; set; }

        public virtual List<TransactionDetail> transactionDetail { get; set; }

        public bool hasTransactions()
        {
            return transactionDetail.Count > 0;
        }

        public void setCode()
        {
            code = name;
        }
    }
}
