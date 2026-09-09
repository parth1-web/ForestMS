using LE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Administration.FilterModel
{
    public class RoleFilter:PaginationFilter
    {
        public string name { get; set; }
    }
}
