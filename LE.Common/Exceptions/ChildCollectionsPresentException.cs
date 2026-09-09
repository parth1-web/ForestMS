using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Exceptions
{
    public class ChildCollectionsPresentException : CustomException
    {
        public ChildCollectionsPresentException(string message = "Child collection is present for specified parent.") : base(message)
        {

        }
    }
}
