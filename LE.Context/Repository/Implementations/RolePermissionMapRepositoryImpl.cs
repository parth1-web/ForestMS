using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using System.Collections.Generic;
using System.Linq;

namespace LE.Context.Repository.Implementations
{
    public class RolePermissionMapRepositoryImpl : BaseRepositoryImpl<RolePermissionMap>, RolePermissionMapRepository
    {
        private readonly AppDbContext _AppDbContext;
        public RolePermissionMapRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        public List<RolePermissionMap> getByPermission(long module_id)
        {
            return _AppDbContext.role_permission_maps.Where(a => a.module_id == module_id).ToList();
        }

        public List<RolePermissionMap> getByRoleId(long role_id)
        {
            return _AppDbContext.role_permission_maps.Where(a => a.role_id == role_id).ToList();
        }
    }
}
