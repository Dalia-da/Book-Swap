using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSwap.Models
{
    public class BookComment
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string UserName { get; set; }

        // [محذوف] public string UserProfileImagePath { get; set; }  <-- الاسطر دي اتشتغلت عشان نقلناها لـ ApplicationUser

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}