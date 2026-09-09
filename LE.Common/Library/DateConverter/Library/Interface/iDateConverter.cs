using LE.Common.Library.DateConverter.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Library.DateConverter.Library.Interface
{
    public interface iDateConverter
    {
        EnglishDate ToAD(string nepali_date);
        NepaliDate ToBS(DateTime english_date, NepaliDate.DateFormats date_format = NepaliDate.DateFormats.mDy);
    }
}
