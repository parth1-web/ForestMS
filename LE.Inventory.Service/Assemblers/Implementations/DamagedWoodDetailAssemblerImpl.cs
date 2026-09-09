using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class DamagedWoodDetailAssemblerImpl : DamagedWoodDetailAssembler
    {
        public void copy(DamagedWoodDetail damagedWoodDetail, DamagedWoodDetailDto damagedWoodDetailDto)
        {
            damagedWoodDetail.damaged_feet_size = damagedWoodDetailDto.damaged_feet_size;
            damagedWoodDetail.damaged_fifth_size = damagedWoodDetailDto.damaged_fifth_size;
            damagedWoodDetail.damaged_first_size = damagedWoodDetailDto.damaged_first_size;
            damagedWoodDetail.damaged_fourth_size = damagedWoodDetailDto.damaged_fourth_size;
            damagedWoodDetail.damaged_second_size = damagedWoodDetailDto.damaged_second_size;
            damagedWoodDetail.damaged_third_size = damagedWoodDetailDto.damaged_third_size;
            damagedWoodDetail.wood_details_id = damagedWoodDetailDto.wood_details_id;
            damagedWoodDetail.setTotalDamagedSize();
            damagedWoodDetail.damaged_wood_details_id = damagedWoodDetailDto.damaged_wood_details_id;
        }
    }
}
