using BookSwap.Models;
using BookSwap.ContextDBConfig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace BookSwap.Controllers
{
    public class LendHistoryController : Controller
    {
        private readonly BookSwapDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LendHistoryController(BookSwapDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var booksWithUsers = (from book in _context.Books
                                  join user in _context.Users on book.UserId equals user.Id
                                  select new
                                  {
                                      BookTitle = book.Title,
                                      UserName = user.Name,
                                      UserEmail = user.Email,
                                      UserImagePath = user.ImagePath,
                                      UserId = user.Id  
                                  }).ToList();

            var viewModel = booksWithUsers.Select(b => new LendHistoryViewModel
            {
                BookTitle = b.BookTitle,
                UserName = b.UserName,
                UserEmail = b.UserEmail,
                UserImagePath = b.UserImagePath,
                UserId = b.UserId  
            }).ToList();

            return View(viewModel);
        }
    }
}