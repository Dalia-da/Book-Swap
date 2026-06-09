
using System.ComponentModel;

namespace BookSwap.Models
{

    public class AddBookViewModel
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int YearPublished { get; set; }
        public string ImagePath { get; set; } 

        [DisplayName("صورة الكتاب")]
        public IFormFile BookImage { get; set; }  

        [DisplayName("ISBN")]
        public string ISBN { get; set; }  
    }
}