using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Billing.Infrastructure.Dto
{
    public class DayCloseDto
    {
        public long day_close_id { get; set; }
        public DateTime eng_close_date { get; set; }
        public string nep_close_date { get; set; }


    }
}
