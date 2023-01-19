using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignInVM
    {
        [Required]
        [Display(Name= "Mail Address")]
        public string Email { get; set; }
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool IsRemember { get; set; }
    }
}
