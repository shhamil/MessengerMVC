using MessengerMVC2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerMVC2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = _userManager.Users
                .Where(u => u.Id != currentUser.Id);

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    (u.DisplayName != null && u.DisplayName.Contains(searchString)));
            }

            return View(users.ToList());
        }
    }
}
