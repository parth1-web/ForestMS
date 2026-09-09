using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Infrastructure.Dto
{
    public class UserDto
    {
        public long user_id { get; set; }
        public long created_by { get; set; }
        public string full_name { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public bool is_active { get; set; } = true;
        public string email { get; set; }
        public string image_path { get; set; }
        public List<long> role_ids { get; set; }

        public string username { get; set; }
        public string password { get; set; }
    }
}
