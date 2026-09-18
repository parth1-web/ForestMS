using LE.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Entities.User
{
    public class UserRole
    {
        [Key]
        public long user_role_id { get; set; }

        [Required]
        public UserType type { get; set; } = UserType.user;

        [Required]
        public long type_id { get; set; }

        [Required]
        public long role_id { get; set; }

        [ForeignKey("role_id")]
        public virtual Role role { get; set; }

    }
}
