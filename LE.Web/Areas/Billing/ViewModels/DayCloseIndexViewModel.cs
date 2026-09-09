using DateConverter.Core.Service_Factory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.ViewModels
{
    public class DayCloseIndexViewModel
    {
        [Display(Name ="Date")]
        public string date { get; set; }= DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public decimal day_counter_sales { get; set; }
        public decimal day_firewood_sales { get; set; }
        public decimal day_wood_sales { get; set; }
        public bool is_closed { get; set; }

        public List<ServiceCount> service_count { get; set; }
    }

    public class ServiceCount
    {
        public LE.Billing.Entities.Service service { get; set; }
        public long service_id { get; set; }
        public decimal qty { get; set; }
    }
}
