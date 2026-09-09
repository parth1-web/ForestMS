using LE.Infrastructure.Dto;

namespace LE.Service.Services.Interface
{
    public interface UserRoleService
    {
        void save(UserRoleDto dto);
        void update(UserRoleDto dto);
    }
}
