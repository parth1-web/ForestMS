using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Billing.Entities
{
    public class Service
    {
        private long _categoryId, _ledgerId;
        private decimal _rate;

        [Key]
        public long service_id { get; set; }

        [Required]
        public long category_id
        {
            get => _categoryId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Category id is invalid.");
                }
                _categoryId = value;
            }
        }

        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal rate
        {
            get => _rate;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Rate is Invalid.");
                }
                _rate = value;
            }
        }

        public DateTime created_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
        public long created_by { get; set; }
        public bool is_enabled { get; set; } = true;

        public decimal tax { get; set; }

        [Required]
        public long ledger_id
        {
            get => _ledgerId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Ledger is Invalid.");
                }
                _ledgerId = value;
            }
        }

        [ForeignKey("category_id")]
        public virtual Ledger ledger { get; set; }

        [ForeignKey("category_id")]
        public virtual ServiceCategory service_category { get; set; }

        public void enable()
        {
            is_enabled = true;
        }

        public void disable()
        {
            is_enabled = false;
        }
    }
}
