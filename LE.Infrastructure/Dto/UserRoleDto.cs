using LE.Common.Enums;

namespace LE.Infrastructure.Dto
{
    public class UserRoleDto
    {
        public long[] role_ids { get; set; }
        public UserType type { get; set; }
        public long type_id { get; set; }
    }
}
