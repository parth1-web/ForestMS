using LE.Inventory.Common.Enums;

namespace LE.Inventory.Infrastructure.Dto
{
    public class StockMovementDetailDto
    {
        public long stock_item_id { get; set; }
        public decimal qty { get; set; }
        public MovementOperation operation { get; set; }
    }
}
