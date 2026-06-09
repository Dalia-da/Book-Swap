using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSwap.Models
{
    public enum UserRole
    {
        Admin,
        User
    }
    public class ApplicationUser : IdentityUser
    {

        public string? Name { get; set; }
     
        public string? Role { get; set; }  

        public string? Address { get; set; }
        [Display(Name = "Image")]
        [DefaultValue("default.png")]
        public string? ImagePath { get; set; }

    

        public ICollection<Book> Books { get; set; }  
        public List<LendHistory> LendBooks { get; set; }  


    }
}
