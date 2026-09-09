using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Billing.Entities
{
    public class ServiceCategory
    {
        [Key]
        public long category_id { get; set; }

        [MaxLength(100)]
        [Required]
        public string name { get; set; }

        public bool is_enabled { get; set; } = true;

        public virtual List<Service> services { get; set; }

        public void enable()
        {
            is_enabled = true;
            services.ForEach(a => a.is_enabled = true);
        }

        public void disable()
        {
            is_enabled = false;
            services.ForEach(a => a.is_enabled = false);
        }

        public bool hasServices() => services.Count > 0;
    }
}
