using LE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Billing.FilterModel
{
    public class ServiceCategoryFilter:PaginationFilter
    {
        public string name { get; set; }
    }
}
