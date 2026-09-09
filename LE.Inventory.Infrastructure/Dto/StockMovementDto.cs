using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class StockMovementDto
    {
        private long _movementTypeId;
        private List<StockMovementDetailDto> stockMovementDetailDtos = new List<StockMovementDetailDto>();

        public StockMovementType movement_type { get; set; }

        [Required]
        public long movement_type_id
        {
            get => _movementTypeId;
            set
            {
                if (value < 1)
                {
                    throw new InvalidValueException("Invalid movement type id.");
                }
                _movementTypeId = value;
            }
        }


        public List<StockMovementDetailDto> getStockMovementDetails() => stockMovementDetailDtos;

        public void addStockMovementDetail(StockMovementDetailDto dto)
        {
            stockMovementDetailDtos.Add(dto);
        }

        public bool isMovementValid() => stockMovementDetailDtos.Count > 0;
    }
}
