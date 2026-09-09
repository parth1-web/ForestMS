using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface PilingAssembler
    {
        void copy(ref Piling piling, PilingDto pilingDto);
    }
}
