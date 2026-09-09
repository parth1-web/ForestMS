using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class StockItemAssemblerImpl : StockItemAssembler
    {
        public void copy(ref StockItem stock_item, StockItemDto stock_item_dto)
        {
            stock_item.stock_item_id = stock_item_dto.stock_item_id;
            stock_item.wood_type_id = stock_item_dto.wood_type_id;
            stock_item.stock_unit_id = stock_item_dto.stock_unit_id;
            stock_item.name = stock_item_dto.name;
            stock_item.threshold = stock_item_dto.threshold;
            stock_item.default_sales_rate = stock_item_dto.default_sales_rate;
            stock_item.is_enabled = stock_item.is_enabled;

        }
    }
}
