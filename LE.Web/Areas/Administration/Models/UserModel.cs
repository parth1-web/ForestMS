using LE.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Administration.Models
{
    public class UserModel
    {
        public long user_id { get; set; }
        public long created_by { get; set; }

        [Display(Name ="Full Name")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Full name  is required")]
        public string full_name { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "At least one address is required")]
        public string address_line_1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string address_line_2 { get; set; }

        [Display(Name = "Primary Contact")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Primary contact number is required")]
        public string primary_contact { get; set; }
        [Display(Name = "Secondary Contact")]
        public string secondary_contact { get; set; }
        [Display(Name = "Status")]
        public bool is_active { get; set; } = true;
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string username { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare(nameof(password),ErrorMessage ="Passwords didnot match.")]
        public string confirm_paswword { get; set; }

        public string image_path { get; set; }

        [Display(Name = "Roles")]
        public List<AssignedRole> roles { get; set; }
    }

    public class AssignedRole
    {
        public long role_id { get; set; }
        public string role_name { get; set; }
        public bool is_checked { get; set; }
    }
}
