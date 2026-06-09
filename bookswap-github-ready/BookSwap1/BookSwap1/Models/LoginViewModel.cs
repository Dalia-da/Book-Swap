
using System.ComponentModel.DataAnnotations;
namespace BookSwap.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? ForgetPasswordUrl { get; set; }  

    }
}
