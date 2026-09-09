using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Helpers
{
    public class AlertHelper
    {
        public static void setMessage(Controller controller, string message, messageType message_type =messageType.success)
        {
            Alert alert = new Alert();
            alert.message = message;
            alert.message_type = message_type;
            controller.TempData["message"] = JsonConvert.SerializeObject(alert);
        }
    }
}
