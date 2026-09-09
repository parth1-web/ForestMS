using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;
using System;

namespace LE.Service.Assembler.Implementations
{
    public class LoginSessionMakerImpl : LoginSessionMaker
    {
        public void copy(LoginSession session, LoginSessionDto session_dto)
        {
            session.date_time = DateTime.Now;
            session.authentication_id = session_dto.authentication_id;
            session.type = session_dto.type;
        }
    }
}
