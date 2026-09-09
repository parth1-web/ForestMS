using System.ComponentModel.DataAnnotations;

namespace LE.Infrastructure.Dto
{
    public class UpdatePasswordDto
    {
        [Required]
        [Range(1,long.MaxValue)]
        public long type_id { get; set; }

        public LE.Common.Enums.UserType type { get; set; } = LE.Common.Enums.UserType.user;

        [Required(AllowEmptyStrings =false,ErrorMessage ="Old password is required.")]
        [Display(Name ="Old Password")]
        public string old_password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string new_password { get; set; }

        [Compare(nameof(new_password),ErrorMessage ="Passwords didnot match.")]
        [Display(Name = "Confirm Password")]
        public string confirm_password { get; set; }
    }
}
