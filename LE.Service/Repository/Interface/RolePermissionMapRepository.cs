using LE.Entities.User;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface RolePermissionMapRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(RolePermissionMap role_permission_map);
        void update(RolePermissionMap role_permission_map);
        void delete(RolePermissionMap role_permission_map);
        List<RolePermissionMap> getAll();
        RolePermissionMap getById(long user_id);
        List<RolePermissionMap> getByRoleId(long role_id);
        List<RolePermissionMap> getByPermission(long module_id);
        IQueryable<RolePermissionMap> getQueryable();
    }
}
