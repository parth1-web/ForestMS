using DateConverter.Core.Service_Factory;
using LE.Service.Services.Interface;
using System;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Service.Services.Implementations
{
    public class DateConverterServiceImpl : DateConverterService
    {
        public DateTime toAD(string nepali_date)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            return dateConverterService.ToAD(nepali_date).getFormattedDate();
        }

        public string toBS(DateTime english_date)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            return dateConverterService.ToBS(english_date, DateFormats.yMd).getFormattedDate();
        }

        public DateTime getDateByTimeZone()
        {
            var dateConverterService = DateFunctionsFactory.getDateFunctionsService();
            return dateConverterService.getDateTimeByTimeZone();
        }
    }
}
