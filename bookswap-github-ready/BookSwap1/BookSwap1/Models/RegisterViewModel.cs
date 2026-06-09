using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookSwap.Models
{
	public class RegisterViewModel
	{
		[Required]
        public string Name { get; set; }
		[Required]
		public string Address {  get; set; }
		[Required,EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Password {  get; set; }
        [Required]

        public string Role { get; set; } 

        [Display(Name = "Image")]
        [DefaultValue("default.png")]

        public string? ImagePath { get; set; }
 
    }
}
