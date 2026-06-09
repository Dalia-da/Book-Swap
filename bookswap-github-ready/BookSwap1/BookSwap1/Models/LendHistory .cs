using System.ComponentModel;

namespace BookSwap.Models
{
    public class LendHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserEmail { get; set; }
 
        public string UserAddress { get; set; }
        public string UserProfileImagePath { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        [DisplayName("ISBN")]
        public string ISBN { get; set; }
        public int BookYearPublished { get; set; }
        public string BookImagePath { get; set; }
        public DateTime LendDate { get; set; }
        public virtual ICollection<BookRating> Ratings { get; set; }
    }

}
