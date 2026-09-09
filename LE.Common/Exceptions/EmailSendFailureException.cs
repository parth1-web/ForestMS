using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Exceptions
{
    public class EmailSendFailureException:Exception
    {
        public EmailSendFailureException(string message):base(message)
        {

        }
    }
}
