using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Infrastructure.Dto
{
    public class RoleDto
    {
        public long role_id { get; set; }
        public string name { get; set; }
        public bool is_active { get; set; }

        public List<long> module_ids { get; set; }
    }
}
