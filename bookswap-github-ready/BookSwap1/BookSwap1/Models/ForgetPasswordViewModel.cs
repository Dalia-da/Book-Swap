using System.ComponentModel.DataAnnotations;

namespace BookSwap.Models
{
    public class ForgetPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

}
