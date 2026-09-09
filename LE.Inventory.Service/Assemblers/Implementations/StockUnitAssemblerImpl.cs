using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class StockUnitAssemblerImpl : StockUnitAssembler
    {
        public void copy(ref StockUnit stock_unit, StockUnitDto stock_unit_dto)
        {
            stock_unit.stock_unit_id = stock_unit_dto.stock_unit_id;
            stock_unit.name = stock_unit_dto.name;
            stock_unit.short_name = stock_unit_dto.short_name;
            stock_unit.is_enabled = stock_unit_dto.is_enabled;
        }
    }
}
