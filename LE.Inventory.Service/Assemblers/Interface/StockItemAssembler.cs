using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface StockItemAssembler
    {
        void copy(ref StockItem stock_item, StockItemDto stock_item_dto);
    }
}
