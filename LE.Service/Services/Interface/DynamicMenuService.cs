using LE.Entities.User;
using LE.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Service.Services.Interface
{
    public interface DynamicMenuService
    {
        DynamicMenu save(DynamicMenuDto dto);
        void update(DynamicMenuDto dto);
        void delete(long menu_id);
    }
}
