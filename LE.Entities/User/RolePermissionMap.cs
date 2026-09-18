using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Entities.User
{
    public class RolePermissionMap
    {
        [Key]
        public long role_permission_map_id { get; set; }

        [Required]
        public long role_id { get; set; }

        public long module_id { get; set; }

        public long? menu_id { get; set; }

        [ForeignKey("role_id")]
        public virtual Role role { get; set; }

        [ForeignKey(nameof(module_id))]
        public virtual Module module { get; set; }

        [ForeignKey(nameof(menu_id))]
        public virtual DynamicMenu menu { get; set; }
    }
}
