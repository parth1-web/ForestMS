using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Administration.Models
{
    public class RoleModel
    {
        public long role_id { get; set; }

        [Required(AllowEmptyStrings =false,ErrorMessage ="Role name is required")]
        public string name { get; set; }
        public bool is_enabled { get; set; } = true;

        public List<PermissionModel> permission_datas { get; set; } = new List<PermissionModel>();
    }
}
