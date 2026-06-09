using BookSwap.Models;

public class BookRating
{
    public int Id { get; set; }
    public int LendHistoryId { get; set; }
    public string UserId { get; set; }
    public int Rating { get; set; }  
    public DateTime CreatedDate { get; set; }

    public LendHistory LendHistory { get; set; }
    public ApplicationUser User { get; set; }
}