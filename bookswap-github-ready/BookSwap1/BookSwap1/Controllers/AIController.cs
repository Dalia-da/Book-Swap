using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BookSwap.ContextDBConfig;
using BookSwap.Models;

namespace BookSwap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIController> _logger;
        private readonly BookSwapDBContext _context;

        public AIController(IHttpClientFactory httpClientFactory, ILogger<AIController> logger, BookSwapDBContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer xai-VWMgKnGt7Vei6woIPrlRxLk0jmaKBLj26vQErDBUnfOsfArFNZ6vVPHTdBCqGP48d9hGNTMVmE4ATKBD");
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query is required.");
            }

            try
            {
                string response = GenerateResponse(query);
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat request with query: {Query}", query);
                return StatusCode(500, "Internal server error occurred. Check logs for details.");
            }
        }

        private string GenerateResponse(string query)
        {
            query = query.ToLower();
            var lendHistories = _context.LendHistories.Include(l => l.Ratings).ToList();
            _logger.LogInformation("Number of LendHistories: {Count}", lendHistories?.Count ?? 0);

            if (lendHistories == null || !lendHistories.Any())
            {
                _logger.LogWarning("No data found in LendHistories.");
                return "لا توجد بيانات في قاعدة البيانات.";
            }

            try
            {
                if (query.Contains("قائمة الكتب"))
                {
                    var bookList = lendHistories
                        .Where(l => !string.IsNullOrEmpty(l.BookTitle) && !string.IsNullOrEmpty(l.BookAuthor))
                        .Select(l => $"{l.BookTitle} by {l.BookAuthor}")
                        .ToList();
                    return bookList.Any() ? string.Join("\n", bookList) : "لا توجد كتب متاحة.";
                }
                else if (query.Contains("أفضل الكتب"))
                {
                    var topBooks = lendHistories
                        .Where(l => l.Ratings != null && l.Ratings.Any())
                        .OrderByDescending(l => l.Ratings.Average(r => r.Rating))
                        .Take(3)
                        .Select(l => $"{l.BookTitle} by {l.BookAuthor} (Rating: {l.Ratings.Average(r => r.Rating):F1})")
                        .ToList();
                    return topBooks.Any() ? string.Join("\n", topBooks) : "لا توجد كتب بتقييمات بعد.";
                }
                else if (query.Contains("كتب جديدة"))
                {
                    var newBooks = lendHistories
                        .Where(l => l.LendDate != null)
                        .OrderByDescending(l => l.LendDate)
                        .Take(3)
                        .Select(l => $"{l.BookTitle} by {l.BookAuthor} (Lent on: {l.LendDate:yyyy-MM-dd})")
                        .ToList();
                    return newBooks.Any() ? string.Join("\n", newBooks) : "لا توجد كتب جديدة.";
                }
                else
                {
                    return "سؤال غير معرف. جرب: 'قائمة الكتب', 'أفضل الكتب', أو 'كتب جديدة'.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateResponse for query: {Query}", query);
                return "حدث خطأ أثناء معالجة الطلب.";
            }
        }
    }
}