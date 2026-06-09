using System.Collections.Generic;

namespace BookSwap.Models
{
    public class BookCategoryViewModel
    {
        public string Genre { get; set; }

        public List<Book> Books { get; set; }
    }
}
