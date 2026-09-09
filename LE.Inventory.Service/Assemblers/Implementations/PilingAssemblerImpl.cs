using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class PilingAssemblerImpl : PilingAssembler
    {
        public void copy(ref Piling piling, PilingDto pilingDto)
        {
            piling.piling_id = pilingDto.piling_id;
            piling.title = pilingDto.title;
            piling.description = pilingDto.description;
            piling.is_enabled = pilingDto.is_enabled;
        }
    }
}
