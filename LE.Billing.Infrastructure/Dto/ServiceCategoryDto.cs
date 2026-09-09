using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Billing.Infrastructure.Dto
{
    public class ServiceCategoryDto
    {
        public long category_id { get; set; }

        [MaxLength(100)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category name is required.")]
        public string name { get; set; }

        public bool is_enabled { get; set; } = true;
    }
}
