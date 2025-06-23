using Microsoft.AspNetCore.SignalR;

namespace MessengerMVC2
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            await Clients.Users(receiverId, senderId)
                .SendAsync("ReceiveMessage", senderId, receiverId, message, DateTime.UtcNow.ToString());
        }
    }
}
