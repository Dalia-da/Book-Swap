using BookSwap.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookSwap.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = user.Id;
                ViewData["UserImagePath"] = user.ImagePath ?? "~/Images/default.png";
            }

            return View();
        }

        public async Task<IActionResult> About()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = user.Id;
                ViewData["UserImagePath"] = user.ImagePath ?? "~/Images/default.png";
            }

            return View();
        }

        public async Task<IActionResult> Contact()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserId"] = user.Id;
                ViewData["UserImagePath"] = user.ImagePath ?? "~/Images/default.png";
            }

            return View();
        }

        [Authorize]  
        public async Task<IActionResult> MyMessages()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");  
            }

            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

     
    }
}
