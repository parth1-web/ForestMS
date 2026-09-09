using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LE.Common.Library
{
    public static class NepaliNumeralHelper
    {
        public static string ConvertNumerals(this string input)
        {
            if (new string[] { "ne-NP" }
                  .Contains(Thread.CurrentThread.CurrentCulture.Name))
            {
                return input.Replace('0', 'o')
                        .Replace('1', '१')
                        .Replace('2', '२')
                        .Replace('3', '३')
                        .Replace('4', '४')
                        .Replace('5', '५')
                        .Replace('6', '६')
                        .Replace('7', '७')
                        .Replace('8', '८')
                        .Replace('9', '९');
            }
            else return input;
        }
    }
}
