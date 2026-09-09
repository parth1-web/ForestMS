using LE.Infrastructure.Dto;

namespace LE.Service.Services.Interface
{
    public interface LoginSessionService
    {
        void save(LoginSessionDto session_dto);
    }
}
