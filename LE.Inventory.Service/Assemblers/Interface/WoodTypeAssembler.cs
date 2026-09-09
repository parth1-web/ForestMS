using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface WoodTypeAssembler
    {
        void copy(WoodType wood_type, WoodTypeDto wood_type_dto);
    }
}
