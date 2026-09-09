using LE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Setup.FilterModel
{
    public class ModuleFilter:PaginationFilter
    {
        public string name { get; set; }
    }
}
