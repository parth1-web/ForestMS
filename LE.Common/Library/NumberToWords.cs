using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LE.Common.Library
{
    public class NumberToWords
    {
        public static string NumberToCurrencyText(decimal number, MidpointRounding midpointRounding)
        {
            number = decimal.Round(number, 2, midpointRounding);

            string wordNumber = string.Empty;
            string[] arrNumber = number.ToString().Split('.');
			
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToText(wholePart);

            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " Rupees and " : " Rupees and ");

         
            if (arrNumber.Length > 1)
            {
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToText(fractionPart);

                wordNumber += (fractionPart == 0 ? " No" : strFarctionPart) + (fractionPart == 1 ? " Paisa" : " Paisa");
            }
            else
                wordNumber += "No Paisa";

            return wordNumber;
        }


        public static string NumberToText(long number)
        {
            StringBuilder wordNumber = new StringBuilder();

            string[] powers = new string[] { "Thousand ", "Million ", "Billion " };
            string[] tens = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            string[] ones = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", 
                                       "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

            if (number == 0) { return "Zero"; }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            long[] groupedNumber = new long[] { 0, 0, 0, 0 };
            int groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (int i = 3; i >= 0; i--)
            {
                long group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Hundred ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }
    }
}