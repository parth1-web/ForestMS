using LE.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Service.Services.Interface
{
    public interface ModuleService
    {
        void save(ModuleDto module_dto);
        void update(ModuleDto module_dto);
        void delete(long module_id);
    }
}
