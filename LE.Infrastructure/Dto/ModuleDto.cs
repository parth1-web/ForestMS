using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Infrastructure.Dto
{
    public class ModuleDto
    {
        public long module_id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Module name is required")]
        public string module_name { get; set; }
        public string module_code { get; set; }

        [MaxLength(50)]
        public string display_icon { get; set; }
    }
}
