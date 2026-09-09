using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Service.Assembler.Implementations
{
    public class ModuleAssemblerImpl : ModuleAssembler
    {
        public void copy(Module entity, ModuleDto dto)
        {
            entity.module_name = dto.module_name;
            entity.module_code = dto.module_code;
            entity.display_icon = dto.display_icon;
        }
    }
}
