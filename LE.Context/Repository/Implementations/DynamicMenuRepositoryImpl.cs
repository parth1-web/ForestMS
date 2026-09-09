using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Context.Repository.Implementations
{
    public class DynamicMenuRepositoryImpl : BaseRepositoryImpl<DynamicMenu>, DynamicMenuRepository
    {
        private readonly AppDbContext _appDbContext;

        public DynamicMenuRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<DynamicMenu> getByModule(long module_id)
        {
            return _appDbContext.dynamic_menus.Where(a => a.module_id == module_id).ToList();
        }

        public DynamicMenu getByModuleAndMenuName(long module_id, string menu_name)
        {
            string trimmedMenuName = menu_name.Trim().Replace(" ", string.Empty).ToLower();

            return _appDbContext.dynamic_menus.Where(a => a.module_id == module_id && (a.menu_name.Trim().Replace(" ", string.Empty).ToLower()).Equals(trimmedMenuName)).SingleOrDefault();
        }

        public List<DynamicMenu> getByParentId(long parent_id)
        {
            return _appDbContext.dynamic_menus.Where(a => a.parent_menu_id == parent_id).ToList();
        }
    }
}
