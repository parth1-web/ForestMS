using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using System.Collections.Generic;

namespace LE.Inventory.Service.Services.Interface
{
    public interface DamagedWoodDetailService
    {
        void save(List<DamagedWoodDetailDto> damagedWoodDetailDtos);
        void update(List<DamagedWoodDetailDto> damagedWoodDetailDtos);
        void delete(long damaged_wood_detail_id);
    
    }
}
