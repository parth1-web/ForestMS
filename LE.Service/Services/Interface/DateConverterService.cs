using System;

namespace LE.Service.Services.Interface
{
    public interface DateConverterService
    {
        string toBS(DateTime english_date);
        DateTime toAD(string nepali_date);
        DateTime getDateByTimeZone();
    }
}
