using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BlazorApp1.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinChatRoom(string chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{chatRoomId}");
        }

        public async Task LeaveChatRoom(string chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{chatRoomId}");
        }

        public async Task SendMessageToRoom(string chatRoomId, string message, string senderName)
        {
            await Clients.Group($"chat_{chatRoomId}").SendAsync("ReceiveMessage", chatRoomId, message, senderName);
        }

        public async Task NotifyTyping(string chatRoomId, string userName)
        {
            await Clients.OthersInGroup($"chat_{chatRoomId}").SendAsync("UserTyping", userName);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}