using System;
using System.ComponentModel.DataAnnotations;

namespace LE.Billing.Infrastructure.Dto
{
    public class ToleDto
    {
        public long tole_id { get; set; }
        [Required(ErrorMessage = "Tole No is required.")]
        [Display(Name ="Tole No")]
        public string tole_no { get; set; }
    }
}
