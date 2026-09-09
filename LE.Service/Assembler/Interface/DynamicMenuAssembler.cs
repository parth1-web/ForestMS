using LE.Entities.User;
using LE.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Service.Assembler.Interface
{
    public interface DynamicMenuAssembler
    {
        void copy(DynamicMenu entity, DynamicMenuDto dto);
    }
}
