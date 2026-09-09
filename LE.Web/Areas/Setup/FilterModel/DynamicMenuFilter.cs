using LE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Setup.FilterModel
{
    public class DynamicMenuFilter : PaginationFilter
    {
        public string name { get; set; }
        public long module_id { get; set; }
    }
}
