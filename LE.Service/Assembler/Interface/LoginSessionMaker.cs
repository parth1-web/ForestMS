using LE.Entities.User;
using LE.Infrastructure.Dto;

namespace LE.Service.Assembler.Interface
{
    public interface LoginSessionMaker
    {
        void copy(LoginSession session, LoginSessionDto session_dto);
    }
}
