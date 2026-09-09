using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Library.DateConverter.Library.Interface
{
    public interface iConversionStartDateData
    {
        Tuple<int[], int[], int> getClosestEnglishDateAndNepaliDate(DateTime english_date);

        Tuple<int, int[]> getClosestEnglishDateAndNepaliDateToAD(int nep_year);
    }
}
