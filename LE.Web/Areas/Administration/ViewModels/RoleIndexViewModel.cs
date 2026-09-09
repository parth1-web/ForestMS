using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Administration.ViewModels
{
    public class RoleIndexViewModel
    {
        public List<RoleDetailModel> role_details { get; set; }
    }

    public class RoleDetailModel
    {
        public long role_id { get; set; }
        public string name { get; set; }
        public bool is_active { get; set; }
        public List<string> permissions { get; set; }
    }
}
