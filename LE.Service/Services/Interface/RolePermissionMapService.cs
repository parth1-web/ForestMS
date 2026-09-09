using LE.Infrastructure.Dto;

namespace LE.Service.Services.Interface
{
    public interface RolePermissionMapService
    {
        void saveOrUpdate(RolePermissionMapDto dto);
        void deletePermissionsByRoleId(long role_id);
    }
}
