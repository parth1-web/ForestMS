using LE.Entities.User;
using LE.Infrastructure.Dto;

namespace LE.Service.Assembler.Interface
{
    public interface AuthenticationMaker
    {
        void copy(Authentication authentication, AuthenticationDto authentication_dto);
    }
}
