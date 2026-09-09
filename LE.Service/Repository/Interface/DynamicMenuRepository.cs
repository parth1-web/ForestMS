using LE.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Service.Repository.Interface
{
    public interface DynamicMenuRepository
    {
        void insert(DynamicMenu dynamic_menu);
        void update(DynamicMenu dynamic_menu);
        void delete(DynamicMenu dynamic_menu);
        DynamicMenu getById(long dynamic_menu_id);
        List<DynamicMenu> getAll();
        List<DynamicMenu> getByModule(long module_id);
        List<DynamicMenu> getByParentId(long parent_id);
        DynamicMenu getByModuleAndMenuName(long module_id, string menu_name);
        IQueryable<DynamicMenu> getQueryable();
    }
}
