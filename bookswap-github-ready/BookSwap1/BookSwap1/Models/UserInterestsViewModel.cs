using Microsoft.AspNetCore.Mvc.Rendering;

public class UserInterestsViewModel
{
    public List<SelectListItem> GenreOptions { get; set; }
    public List<SelectListItem> AuthorOptions { get; set; }
    public string SelectedGenre { get; set; }
    public string SelectedAuthor { get; set; }
}
