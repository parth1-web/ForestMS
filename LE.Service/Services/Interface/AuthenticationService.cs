using LE.Common.Enums;
using LE.Entities.User;
using LE.Infrastructure.Dto;

namespace LE.Service.Services.Interface
{
    public interface AuthenticationService
    {
        void enable(long type_id, UserType type = UserType.user);
        void disable(long type_id, UserType type = UserType.user);
        void save(AuthenticationDto authentication_dto);
        void updateUsername(string new_name, long type_id, UserType type = UserType.user);
        void updatePassword(UpdatePasswordDto dto);

        Authentication validateUser(string username, string password);
    }
}
