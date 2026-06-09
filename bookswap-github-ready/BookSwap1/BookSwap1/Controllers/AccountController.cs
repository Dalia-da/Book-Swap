using BookSwap.ContextDBConfig;
using BookSwap.Models;
using BookSwap.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookSwap.Controllers
{
    public class AccountController : Controller
    {
       
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly BookSwapDBContext _context;
            private readonly IWebHostEnvironment _environment;
            private readonly EmailSender _emailSender;

           
            public AccountController(BookSwapDBContext context,
                                     UserManager<ApplicationUser> userManager,
                                     SignInManager<ApplicationUser> signInManager,
                                     RoleManager<IdentityRole> roleManager,
                                     IWebHostEnvironment environment,
                                     EmailSender emailSender)
            {
                _context = context;
                _userManager = userManager;
                _signInManager = signInManager;
                _roleManager = roleManager;
                _environment = environment;
                _emailSender = emailSender;
            }
            [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                string body = $"يرجى إعادة تعيين كلمة المرور عبر الضغط على الرابط التالي: <a href='{callbackUrl}'>إعادة تعيين</a>";
                await _emailSender.SendEmailAsync(email, "إعادة تعيين كلمة المرور", body);

                return RedirectToAction("ForgetPasswordConfirmation");
            }

            ModelState.AddModelError("", "البريد الإلكتروني غير صحيح.");
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var model = new ResetPasswordViewModel { Token = token, UserId = userId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation() => View();
        public IActionResult ForgetPasswordConfirmation() => View();
      
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register, IFormFile Images)
        {
            if (register == null)
                return View(register);

            if (Images == null || Images.Length == 0)
            {
                register.ImagePath = "/Images/default.png";
            }
            else
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "Images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Images.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Images.CopyToAsync(fileStream);
                }

                register.ImagePath = "/Images/" + uniqueFileName;
            }

            if (!ModelState.IsValid)
                return View(register);

            var user = new ApplicationUser
            {
                Name = register.Name,
                Address = register.Address,
                Email = register.Email,
                UserName = register.Email,
                ImagePath = register.ImagePath,
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new IdentityRole("User"));

                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(register);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            var model = new LoginViewModel
            {
                ForgetPasswordUrl = Url.Action("ForgetPassword", "Account")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel Login, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Login.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(Login.Email, Login.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                            return LocalRedirect(returnUrl);
                        return RedirectToAction("index", "home");
                    }
                }
                ModelState.AddModelError("", "Invalid Login Attempt");
            }
            return View(Login);
        }

        public IActionResult AdminDashboard() => View();
        public IActionResult UserDashboard() => View();

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var viewModel = new RegisterViewModel
            {
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
                ImagePath = user.ImagePath
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index");

            return View("Error");
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new RegisterViewModel
            {
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Name);
                if (user == null)
                    return NotFound();

                user.Name = model.Name;
                user.Address = model.Address;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var action = HttpContext.Request.Form["action"];
                    return action == "Index"
                        ? RedirectToAction("Index", "Account")
                        : RedirectToAction("Edit", "Account");
                }
                return View(model);
            }

            return View(model);
        }

        private ApplicationUser GetUser()
        {
            var userId = _userManager.GetUserId(User);
            return _userManager.FindByIdAsync(userId).Result;
        }

        public async Task<IActionResult> Profile(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var lendBooks = _context.LendHistories
                .Where(l => l.UserId == userId)
                .Select(l => new LendHistory
                {
                    BookTitle = l.BookTitle,
                    BookAuthor = l.BookAuthor,
                    BookYearPublished = l.BookYearPublished,
                    BookImagePath = l.BookImagePath,
                    ISBN = l.ISBN
                })
                .ToList();

            ViewData["UserImagePath"] = user.ImagePath ?? "/images/default.png";

            var model = new ProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                ImagePath = user.ImagePath ?? "/images/default.png",
                LendBooks = lendBooks,
                IsCurrentUser = userId == currentUserId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
                return NotFound();

            var book = await _context.LendHistories.FirstOrDefaultAsync(l => l.ISBN == isbn);
            if (book == null)
                return NotFound();

            _context.LendHistories.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", new { userId = book.UserId });
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
    }
}
