using LE.Common.Library.DateConverter.Entity;
using LE.Common.Library.DateConverter.Library.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Library.DateConverter.Library
{
    public class FiscalYearFunctions : iFiscalYearFunctions
    {
        iDateConverter dateConverter;
        iNepaliDateData nepaliDateArray;
        public FiscalYearFunctions(iDateConverter _dateConverter, iNepaliDateData _nepaliDateData)
        {
            dateConverter = _dateConverter;
            nepaliDateArray = _nepaliDateData;
        }
        public int getFiscalYear(DateTime givenDate)
        {
            // first find year
            if (givenDate == null)
            {
                givenDate = DateTime.Now;
            }

            NepaliDate nepaliDate = dateConverter.ToBS(givenDate);
            //System.String[] userDateParts = dt.Split(new[] { "-" }, System.StringSplitOptions.None);
            //int Month = int.Parse(userDateParts[0]);
            //int Day = int.Parse(userDateParts[1]);
            //int Year = int.Parse(userDateParts[2]);

            //int first = int.Parse(userDateParts[2]);
            //int second = int.Parse(userDateParts[2]);
            int Month = nepaliDate.npMonth;
            int Day = nepaliDate.npDay;
            int Year = nepaliDate.npYear;
            if (Month >= 4)
            {
                return Year;
            }
            else
            {
                return Year - 1;
            }
        }

        public string getLastDateOfFiscalYear(int fiscal_year)
        {
            if (fiscal_year < 1000)
            {
                return "";
            }
            // return new System.DateTime(fiscal_year + 1, 3, nepaliDateArray.getLastDayOfMonthNep(fiscal_year + 1, 3)).ToShortDateString();
            int return_year = fiscal_year + 1;
            return "03" + "-" + nepaliDateArray.getLastDayOfMonthNep(fiscal_year + 1, 3) + "-" + return_year;
        }

        public DateTime getStartDateOfFiscalYear(int fiscal_year)
        {
            return new DateTime(fiscal_year, 4, 1);
        }
    }
}
