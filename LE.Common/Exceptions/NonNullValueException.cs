using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Exceptions
{
    public class NonNullValueException : CustomException
    {
        public NonNullValueException(string message = "Value cannot be null") : base(message)
        {

        }
    }
}
