using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace MessengerMVC2
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"SignalR User ID resolved: {userId}");
            return userId;
        }
    }
}
