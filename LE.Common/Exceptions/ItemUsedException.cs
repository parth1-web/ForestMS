using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Exceptions
{
    public class ItemUsedException : CustomException
    {
        public ItemUsedException(string message = "Specified item has already been used.") : base(message)
        {

        }
    }
}
