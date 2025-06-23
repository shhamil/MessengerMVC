using MessengerMVC2.Data;
using MessengerMVC2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace MessengerMVC2.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var userMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            var chatPartners = userMessages
                .Select(m => m.SenderId == currentUser.Id ? m.Receiver : m.Sender)
                .Where(u => u.Id != currentUser.Id)
                .DistinctBy(u => u.Id)
                .ToList();

            return View(chatPartners);
        }


        public async Task<IActionResult> Conversation(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var otherUser = await _userManager.FindByIdAsync(userId);

            if (otherUser == null)
            {
                return NotFound();
            }

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == userId) ||
                           (m.SenderId == userId && m.ReceiverId == currentUser.Id))
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            ViewBag.OtherUser = otherUser;
            return View(messages);
        }


        [HttpPost]
        public async Task<IActionResult> Create(string receiverId, string content)
        {
            var sender = await _userManager.GetUserAsync(User);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            if (receiver == null)
            {
                return NotFound();
            }

            var message = new Message
            {
                Content = content,
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ChatHub>>();
            await hubContext.Clients.Users(receiverId, sender.Id)
                .SendAsync("ReceiveMessage", sender.Id, receiverId, content, DateTime.UtcNow.ToString());

            return Ok();
        }
    }
}
