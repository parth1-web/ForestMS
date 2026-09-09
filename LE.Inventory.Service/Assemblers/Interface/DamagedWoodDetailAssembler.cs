using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface DamagedWoodDetailAssembler
    {
        void copy(DamagedWoodDetail damagedWoodDetail, DamagedWoodDetailDto damagedWoodDetailDto);
    }
}
