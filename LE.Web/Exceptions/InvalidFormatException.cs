using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LE.Web.Exceptions
{
    public class InvalidFormatException : Exception
    {
       public InvalidFormatException(string message = "Invalid File Format") : base(message)
        {

        }
    }
}
