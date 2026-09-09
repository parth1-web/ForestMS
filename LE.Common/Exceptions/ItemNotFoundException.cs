using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Exceptions
{
    public class ItemNotFoundException : CustomException
    {
        public ItemNotFoundException(string message = "Item doesnot exist.") : base(message)
        {

        }
    }
}
