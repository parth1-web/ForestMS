using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface StockMovementAssembler
    {
        void copy(StockMovement stock_movement, StockMovementDto stock_movement_dto);
    }
}
