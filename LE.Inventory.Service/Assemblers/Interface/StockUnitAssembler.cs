using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface StockUnitAssembler
    {
        void copy(ref StockUnit stock_unit, StockUnitDto stock_unit_dto);
    }
}
