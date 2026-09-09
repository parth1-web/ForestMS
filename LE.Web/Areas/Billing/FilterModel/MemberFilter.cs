using LE.Web.Models;

namespace LE.Web.Areas.Billing.FilterModel
{
    public class MemberFilter:PaginationFilter
    {
        public string name { get; set; }
    }
}
