using LE.Infrastructure.Dto;

namespace LE.Service.Services.Interface
{
    public interface UserService
    {
        void save(UserDto user_dto);
        void update(UserDto user_dto);
        void enable(long user_id);
        void disable(long user_id);
    }
}
