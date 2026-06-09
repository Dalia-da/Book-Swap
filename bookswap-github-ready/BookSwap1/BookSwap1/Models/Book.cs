
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using BookSwap.Models;
namespace BookSwap.Models
{
    public class Book
    {

        public int Id { get; set; }  
        public string Title { get; set; }  
        public string Author { get; set; }  
        public string Genre { get; set; }  
        public int YearPublished { get; set; }  

        [DisplayName("ISBN")]
        public string ISBN { get; set; }  
        public bool IsLended { get; set; }
        public string Status { get; set; } = "Available";
        public string ImagePath { get; set; } 

        public string? UserId { get; set; }
        public List<BookComment> Comments { get; set; }  
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }  

    }
}