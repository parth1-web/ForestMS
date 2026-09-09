using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Account.Entities
{
    public class AccountSettings
    {
        [Key]
        public long settings_id { get; set; }

        [Required]
        public string key { get; set; }

        [Required]
        public long value { get; set; }
    }
}
