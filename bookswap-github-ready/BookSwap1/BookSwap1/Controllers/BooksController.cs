using Microsoft.AspNetCore.Mvc;
using BookSwap.Models;
using System.Linq;
using BookSwap.ContextDBConfig;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics;

namespace BookSwap.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookSwapDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(BookSwapDBContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookViewModel bookModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (_context.Books.Any(b => b.ISBN == bookModel.ISBN))
            {
                ModelState.AddModelError("ISBN", "This ISBN already exists in the system.");
                return View(bookModel);
            }

            if (!ModelState.IsValid)
            {
                if (bookModel.BookImage == null || bookModel.BookImage.Length == 0)
                {
                    bookModel.ImagePath = "/BookImage/default.png";
                }
                else
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "BookImage");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(bookModel.BookImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await bookModel.BookImage.CopyToAsync(fileStream);
                    }

                    bookModel.ImagePath = "/BookImage/" + uniqueFileName;
                }

                var book = new Book
                {
                    Title = bookModel.Title,
                    ISBN = bookModel.ISBN,
                    UserId = currentUser.Id,
                    Author = bookModel.Author,
                    Genre = bookModel.Genre,
                    YearPublished = bookModel.YearPublished,
                    ImagePath = bookModel.ImagePath
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return RedirectToAction("BookList");
            }

            return View(bookModel);
        }

        public IActionResult BookList()
        {
            var books = _context.Books.ToList();

            var bookViewModels = books.Select(b => new AddBookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genre = b.Genre,
                YearPublished = b.YearPublished,
                ISBN = b.ISBN,
                ImagePath = b.ImagePath
            }).ToList();

            return View(bookViewModels);
        }

        [HttpGet]
        public IActionResult EditBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            var model = new AddBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                YearPublished = book.YearPublished,
                ImagePath = book.ImagePath
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditBook(AddBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var book = _context.Books.Find(model.Id);
                if (book != null)
                {
                    book.Title = model.Title;
                    book.Author = model.Author;
                    book.Genre = model.Genre;
                    book.YearPublished = model.YearPublished;

                    if (model.BookImage != null)
                    {
                        if (!string.IsNullOrEmpty(book.ImagePath))
                        {
                            string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BookImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            model.BookImage.CopyTo(fileStream);
                        }

                        book.ImagePath = "/images/" + uniqueFileName;
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("BookList");
            }
                            
            return View(model);
        }

        public IActionResult BookCategories(string searchTerm)
        {
            var books = _context.Books.ToList();

            var groupedBooks = books
                .GroupBy(b => b.Genre)
                .Select(group => new BookCategoryViewModel
                {
                    Genre = group.Key,
                    Books = group.ToList()
                })
                .ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                groupedBooks = groupedBooks
                    .Select(group => new BookCategoryViewModel
                    {
                        Genre = group.Genre,
                        Books = group.Books
                            .Where(b =>
                                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                b.ISBN.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                b.YearPublished.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            .ToList()
                    })
                    .Where(group => group.Books.Any())
                    .ToList();
            }

            return View(groupedBooks);
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddComment(int BookId, string CommentText)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "يرجى تسجيل الدخول لإضافة تعليق.";
                return RedirectToAction("Details", new { id = BookId });
            }

            if (string.IsNullOrWhiteSpace(CommentText))
            {
                TempData["Message"] = "لا يمكن إضافة تعليق فارغ.";
                return RedirectToAction("Details", new { id = BookId });
            }

            var user = await _userManager.GetUserAsync(User); // جلب بيانات اليوزر
            var comment = new BookComment
            {
                BookId = BookId,
                Text = CommentText,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = user?.Name ?? "Anonymous", // استخدام الاسم لو موجود، لو لا "Anonymous"
                CreatedDate = DateTime.Now
            };

            _context.BookComments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "تم إضافة التعليق بنجاح!";
            return RedirectToAction("Details", new { id = BookId });
        }

        public async Task<IActionResult> LendBook(int bookId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            var book = _context.Books.Find(bookId);
            if (book == null || user == null)
            {
                return NotFound();
            }

            var lendHistory = new LendHistory
            {
                UserId = userId,
                Name = user.Name,
                UserEmail = user.Email,
                BookId = book.Id,
                BookTitle = book.Title,
                ISBN = book.ISBN,
                BookAuthor = book.Author,
                BookYearPublished = book.YearPublished,
                BookImagePath = book.ImagePath,
                LendDate = DateTime.Now,
                UserAddress = user.Address,
                UserProfileImagePath = user.ImagePath
            };

            _context.LendHistories.Add(lendHistory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Account", new { userId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (book.UserId != currentUser.Id)
            {
                return Forbid();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Account", new { userId = currentUser.Id });
        }

        public IActionResult LendHistory(string searchTerm)
        {
            var lendHistoriesQuery = _context.LendHistories
                .Include(lh => lh.Ratings)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                lendHistoriesQuery = lendHistoriesQuery.Where(lh =>
                    lh.BookTitle.ToLower().Contains(searchTerm) ||
                    lh.BookAuthor.ToLower().Contains(searchTerm) ||
                    lh.ISBN.ToLower().Contains(searchTerm)
                );
            }

            var lendHistories = lendHistoriesQuery.ToList();
            return View(lendHistories);
        }

        [HttpPost]
        public ActionResult DeleteLendHistory(int lendHistoryId)
        {
            var lendHistory = _context.LendHistories.Find(lendHistoryId);

            if (lendHistory != null && lendHistory.UserId == _userManager.GetUserId(User))
            {
                _context.LendHistories.Remove(lendHistory);

                var isBookStillLent = _context.LendHistories.Any(lh => lh.BookId == lendHistory.BookId);
                if (!isBookStillLent)
                {
                    var book = _context.Books.Find(lendHistory.BookId);
                    if (book != null)
                    {
                        _context.Books.Remove(book);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("LendHistory", "Books");
            }
            else
            {
                return RedirectToAction("LendHistory", "Books");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(int lendHistoryId, int rating)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _userManager.GetUserId(User);
            var lendHistory = await _context.LendHistories
                .Include(lh => lh.Ratings)
                .FirstOrDefaultAsync(lh => lh.Id == lendHistoryId);

            if (lendHistory == null || rating < 1 || rating > 5)
            {
                return BadRequest("Invalid request.");
            }

            // تحقق لو المستخدم عنده تقييم مسجل بالفعل
            var existingRating = lendHistory.Ratings?.FirstOrDefault(r => r.UserId == userId);
            if (existingRating != null)
            {
                existingRating.Rating = rating;
                existingRating.CreatedDate = DateTime.Now;
            }
            else
            {
                var newRating = new BookRating
                {
                    LendHistoryId = lendHistoryId,
                    UserId = userId,
                    Rating = rating,
                    CreatedDate = DateTime.Now
                };
                _context.BookRatings.Add(newRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("LendHistory");
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