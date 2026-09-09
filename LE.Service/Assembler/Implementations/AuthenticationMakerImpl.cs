using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;

namespace LE.Service.Assembler.Implementations
{
    public class AuthenticationMakerImpl : AuthenticationMaker
    {
        public void copy(Authentication authentication, AuthenticationDto authentication_dto)
        {
            authentication.is_enabled = authentication_dto.is_active;
            authentication.username = authentication_dto.username;
            authentication.type = authentication_dto.type;
            authentication.type_id = authentication_dto.type_id;
        }
    }
}
