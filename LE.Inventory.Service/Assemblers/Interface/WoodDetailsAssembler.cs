using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface WoodDetailsAssembler
    {
        void copy(WoodDetails wood_details, WoodDetailsDto wood_details_dto);
    }
}
