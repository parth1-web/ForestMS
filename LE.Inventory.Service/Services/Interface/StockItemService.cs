using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface StockItemService
    {
        void save(StockItemDto stock_item_dto);
        void update(StockItemDto stock_item_dto);
        void delete(long stock_item_id);
        void enable(long stock_item_id);
        void disable(long stock_item_id);
    }
}
