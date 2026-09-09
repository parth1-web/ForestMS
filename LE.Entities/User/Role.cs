using LE.Common.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Entities.User
{
    public class Role
    {
        private string _name;

        [Key]
        public long role_id { get; set; }

        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Role name must be specified.");
                }
                _name = value;
            }
        }

        public bool is_enabled { get; set; } = true;

        public virtual List<RolePermissionMap> permissions { get; set; } = new List<RolePermissionMap>();

        public bool isPermissionsAssigned() => permissions.Count > 0;

        public void enable()
        {
            is_enabled = true;
        }

        public void disable()
        {
            is_enabled = false;
        }
    }
}
