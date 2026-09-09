using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Billing.Models
{
    public class ServiceCategoryModel
    {
        public long category_id { get; set; }

        [Display(Name = "Category Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category Name is required")]
        public string name { get; set; }
    }
}
