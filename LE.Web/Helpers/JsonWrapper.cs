using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Helpers
{
    public class JsonWrapper
    {
        public static string buildSuccessJson(object data)
        {
            var apiData = new { data };
            return JsonConvert.SerializeObject(apiData);
        }

        public static string buildErrorJson(string error)
        {
            var apiMessage = new { error };
            return JsonConvert.SerializeObject(apiMessage);
        }
    }
}
