using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please enter username.")]
        [Display(Name ="Username")]
        public string username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter password.")]
        [Display(Name ="Password")]
        public string password { get; set; }

        public bool remember_me { get; set; }
    }
}
