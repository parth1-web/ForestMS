using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Administration.Models
{
    public class PermissionModel
    {
        public string module_name { get; set; }

        public long module_id { get; set; }
        public bool is_checked { get; set; }
    }
}
