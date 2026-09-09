using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface StockUnitService
    {
        void save(StockUnitDto stock_unit_dto);
        void update(StockUnitDto stock_unit_dto);
        void delete(long stock_unit_id);
        void enable(long stock_unit_id);
        void disable(long stock_unit_id);
    }
}
