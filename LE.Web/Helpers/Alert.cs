using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Helpers
{
    public class Alert
    {
        public string message { get; set; }
        public messageType message_type { get; set; }
        
    }
    public enum messageType
    {
        success,
        error
    }
}
