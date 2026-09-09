using LE.Common.Enums;

namespace LE.Infrastructure.Dto
{
    public class AuthenticationDto
    {
        public string username { get; set; }
        public string password { get; set; }
        public long type_id { get; set; }
        public  UserType type { get; set; }
        public bool is_active { get; set; } = true;
    }
}
