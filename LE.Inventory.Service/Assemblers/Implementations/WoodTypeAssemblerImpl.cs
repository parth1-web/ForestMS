using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class WoodTypeAssemblerImpl : WoodTypeAssembler
    {
        public void copy(WoodType wood_type, WoodTypeDto wood_type_dto)
        {
            wood_type.wood_type_id = wood_type_dto.wood_type_id;
            wood_type.name = wood_type_dto.name;
            wood_type.default_sales_rate = wood_type_dto.default_sales_rate;
            wood_type.is_enabled = wood_type_dto.is_enabled;
        }
    }
}
