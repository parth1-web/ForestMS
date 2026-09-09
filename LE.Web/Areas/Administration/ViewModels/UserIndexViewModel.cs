using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Administration.ViewModels
{
    public class UserIndexViewModel
    {
        public List<UserDetailModel> user_details { get; set; }
        public long logged_in_authentication_id { get; set; }
    }

    public class UserDetailModel
    {
        public long user_id { get; set; }
        public string full_name { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public string email { get; set; }
        public bool is_active { get; set; }
        public DateTime created_date { get; set; }

        public string username { get; set; }
        public List<string> roles { get; set; }

        public long authentication_id { get; set; }
    }
}
