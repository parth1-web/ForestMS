using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Infrastructure.Dto
{
    public class RolePermissionMapDto
    {
        private List<long> _permissions = new List<long>();
        
        public long role_id { get; set; }
        public List<long> module_ids { get => _permissions; }

        public void addPermission(long module_id)
        {
            if (!_permissions.Contains(module_id))
            {
                _permissions.Add(module_id);
            }
        }

        public void removePermission(long module_id)
        {
            if (_permissions.Contains(module_id))
            {
                _permissions.Remove(module_id);
            }
        }

        public void removeAllPermissions()
        {
            _permissions = new List<long>();
        }
    }
}
