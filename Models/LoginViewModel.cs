using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email can not empty")]
        public string email { get; set; }
        [Required(ErrorMessage = "Email can not empty")]
        public string password { get; set; }
    }
}
