using LE.Common.Enums;

namespace LE.Infrastructure.Dto
{
    public class LoginSessionDto
    {
        public long authentication_id { get; set; }
        public SessionType type { get; set; } = SessionType.login;
    }
}
