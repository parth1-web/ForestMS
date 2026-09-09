using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Billing.Models
{
    public class ServiceModel
    {
        public long service_id { get; set; }

        [Display(Name = "Service Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Service Title is required")]
        public string name { get; set; }


        [Display(Name = "Category")]
        public long category_id { get; set; }

        [Display(Name = "Ledger")]
        public long ledger_id { get; set; }

        [Display(Name = "Rate")]
        public decimal rate { get; set; }

        [Display(Name = "Tax Percent")]
        public decimal tax { get; set; }
    }
}
