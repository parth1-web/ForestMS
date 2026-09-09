using LE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Accounting.FilterModel
{
    public class LedgerGroupFilter : PaginationFilter
    {
        public string name { get; set; }
    }
}
