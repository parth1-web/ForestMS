using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Models
{
    public class PaginationFilter
    {
        public int page { get; set; } = 1;
        public int number_of_rows { get; set; } = 10;
    }
}
