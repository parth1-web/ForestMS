using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using DateConverter.Core.Service_Factory;

namespace LE.Inventory.Entities
{
    public class StockMovement
    {
        private long _movementTypeId;

        [Key]
        public long stock_movement_id { get; set; }

        [Required]
        public DateTime movement_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required]
        [MaxLength(15)]
        public string  nep_movement_date { get; set; }

        [Required]
        public StockMovementType movement_type { get; set; }

        [Required]
        public long movement_type_id
        {
            get => _movementTypeId;
            set
            {
                if (value<1)
                {
                    throw new InvalidValueException("Invalid movement type id.");
                }
                _movementTypeId = value;
            }
        }
    }
}
