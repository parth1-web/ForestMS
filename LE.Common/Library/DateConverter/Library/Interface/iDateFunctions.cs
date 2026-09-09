using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Library.DateConverter.Library.Interface
{
    public interface iDateFunctions
    {
        bool IsLeapYear(int english_year);
        string GetNepaliMonth(int month);
        string GetEnglishMonth(int month);
        string GetDayOfWeek(int day);
        double GetUnixTimestamp(DateTime gDate);
        DateTime FormatUnixTime(double timestamp);
        DateTime getDateTimeByTimeZone(string timeZone = "");
    }
}
