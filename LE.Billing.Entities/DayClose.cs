using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Billing.Entities
{
    public class DayClose
    {
        [Key]
        public long day_close_id { get; set; }

        [Required]
        public string nep_close_date { get; set; }

        [Required]
        public DateTime eng_close_date { get; set; }
    }
}
