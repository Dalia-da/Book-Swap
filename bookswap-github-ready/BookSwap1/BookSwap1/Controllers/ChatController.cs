using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookSwap.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookSwap.ContextDBConfig;

namespace BookSwap.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookSwapDBContext _context;

        public ChatController(UserManager<ApplicationUser> userManager, BookSwapDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(int messageId, string senderId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != senderId)
            {
                return Json(new { success = false, message = "You can only delete your own messages" });
            }

             var message = _context.Chats
                .Where(c => c.SenderId == senderId && _context.Chats.ToList().IndexOf(c) == messageId)
                .FirstOrDefault();
            if (message != null)
            {
                _context.Chats.Remove(message);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Message deleted" });
            }
            return Json(new { success = false, message = "Message not found" });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditMessage(int messageId, string senderId, string newMessage)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != senderId)
            {
                return Json(new { success = false, message = "You can only edit your own messages" });
            }

            var message = _context.Chats
                .Where(c => c.SenderId == senderId && _context.Chats.ToList().IndexOf(c) == messageId)
                .FirstOrDefault();
            if (message != null)
            {
                message.Message = newMessage;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Message edited" });
            }
            return Json(new { success = false, message = "Message not found" });
        }
        [Authorize]
        public async Task<IActionResult> Chat(string receiverId)
        {
            if (string.IsNullOrEmpty(receiverId))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReceiverId = receiverId;
            ViewBag.CurrentUserId = _userManager.GetUserId(User);

             var receiver = await _userManager.FindByIdAsync(receiverId);
            ViewBag.ReceiverName = receiver != null && !string.IsNullOrEmpty(receiver.Name) ? receiver.Name : "Unknown User";
             System.Diagnostics.Debug.WriteLine($"Receiver Name: {ViewBag.ReceiverName}, Receiver Id: {receiverId}");

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage(string receiverId, string message)
        {
            if (string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, message = "Invalid input" });
            }

            var senderId = _userManager.GetUserId(User);
            var chat = new Chat
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                SentDate = DateTime.Now
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Message sent" });
        }

        [Authorize]
        public IActionResult GetMessages(string receiverId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var messages = _context.Chats
                .Where(c => (c.SenderId == currentUserId && c.ReceiverId == receiverId) || (c.SenderId == receiverId && c.ReceiverId == currentUserId))
                .OrderBy(c => c.SentDate)
                .Select(c => new { senderId = c.SenderId, message = c.Message, sentDate = c.SentDate })
                .ToList();
            return Json(messages);
        }

        [Authorize]
        public IActionResult GetUserName(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                string name = !string.IsNullOrEmpty(user.Name) ? user.Name : "Unknown";
                return Json(new { name = name });
            }
            return Json(new { name = "Unknown" });
        }
    }
}