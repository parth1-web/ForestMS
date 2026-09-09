using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Library.DateConverter.Library.Interface
{
    public interface iFiscalYearFunctions
    {
        string getLastDateOfFiscalYear(int fiscal_year);
        int getFiscalYear(DateTime givenDate);
        DateTime getStartDateOfFiscalYear(int fiscal_year);
    }
}
