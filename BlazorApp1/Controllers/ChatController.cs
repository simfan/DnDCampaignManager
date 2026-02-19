using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }

        // GET: api/Chat/rooms
        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetChatRooms()
        {
            var userId = GetCurrentUserId();

            var rooms = await _context.Set<ChatRoom>()
                .Include(r => r.Members)
                .ThenInclude(m => m.User)
                .Include(r => r.Messages)
                .Include(r => r.Campaign)
                .Where(r => r.Members.Any(m => m.UserId == userId))
                .OrderByDescending(r => r.LastMessageAt ?? r.CreatedAt)
                .ToListAsync();

            var roomDtos = rooms.Select(r =>
            {
                var lastMessage = r.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                var member = r.Members.FirstOrDefault(m => m.UserId == userId);
                var unreadCount = member?.LastReadAt != null
                    ? r.Messages.Count(m => m.SentAt > member.LastReadAt && m.SenderId != userId)
                    : r.Messages.Count(m => m.SenderId != userId);

                return new ChatRoomDto
                {
                    ChatRoomId = r.ChatRoomId,
                    Name = r.Name,
                    Type = r.Type.ToString(),
                    CampaignId = r.CampaignId,
                    CampaignName = r.Campaign?.Name ?? "",
                    CreatedAt = r.CreatedAt,
                    LastMessageAt = r.LastMessageAt,
                    LastMessage = lastMessage?.Content,
                    UnreadCount = unreadCount,
                    MemberNames = r.Members.Select(m => m.User.UserName ?? "Unknown").ToList()
                };
            }).ToList();

            return Ok(roomDtos);
        }

        // GET: api/Chat/rooms/{id}/messages
        [HttpGet("rooms/{id}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetMessages(int id)
        {
            var userId = GetCurrentUserId();

            var room = await _context.Set<ChatRoom>()
                .Include(r => r.Members)
                .Include(r => r.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(r => r.ChatRoomId == id);

            if (room == null)
                return NotFound();

            if (!room.Members.Any(m => m.UserId == userId))
                return Forbid();

            var messageDtos = room.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageDto
                {
                    ChatMessageId = m.ChatMessageId,
                    ChatRoomId = m.ChatRoomId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.UserName ?? "Unknown",
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsEdited = m.IsEdited,
                    EditedAt = m.EditedAt,
                    IsOwnMessage = m.SenderId == userId
                })
                .ToList();

            return Ok(messageDtos);
        }

        // POST: api/Chat/rooms
        [HttpPost("rooms")]
        public async Task<ActionResult<ChatRoomDto>> CreateChatRoom(CreateChatRoomDto dto)
        {
            var userId = GetCurrentUserId();

            if (!Enum.TryParse<ChatRoomType>(dto.Type, out var roomType))
                return BadRequest("Invalid room type");

            // Validate campaign membership if campaign-wide chat
            if (roomType == ChatRoomType.CampaignWide && dto.CampaignId.HasValue)
            {
                var isMember = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == dto.CampaignId.Value && cm.UserId == userId);

                if (!isMember)
                    return Forbid();

                // Check if campaign-wide chat already exists
                var existingRoom = await _context.Set<ChatRoom>()
                    .FirstOrDefaultAsync(r => r.CampaignId == dto.CampaignId && r.Type == ChatRoomType.CampaignWide);

                if (existingRoom != null)
                    return BadRequest("Campaign-wide chat already exists");
            }

            var room = new ChatRoom
            {
                Name = dto.Name,
                Type = roomType,
                CampaignId = dto.CampaignId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<ChatRoom>().Add(room);
            await _context.SaveChangesAsync();

            // Add members
            var memberIds = new List<string>(dto.MemberUserIds);
            if (!memberIds.Contains(userId))
                memberIds.Add(userId);

            foreach (var memberId in memberIds)
            {
                _context.Set<ChatRoomMember>().Add(new ChatRoomMember
                {
                    ChatRoomId = room.ChatRoomId,
                    UserId = memberId,
                    JoinedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            // Load the room with members for the response
            room = await _context.Set<ChatRoom>()
                .Include(r => r.Members)
                .ThenInclude(m => m.User)
                .Include(r => r.Campaign)
                .FirstAsync(r => r.ChatRoomId == room.ChatRoomId);

            var roomDto = new ChatRoomDto
            {
                ChatRoomId = room.ChatRoomId,
                Name = room.Name,
                Type = room.Type.ToString(),
                CampaignId = room.CampaignId,
                CampaignName = room.Campaign?.Name ?? "",
                CreatedAt = room.CreatedAt,
                MemberNames = room.Members.Select(m => m.User.UserName ?? "Unknown").ToList()
            };

            return CreatedAtAction(nameof(GetChatRooms), new { id = room.ChatRoomId }, roomDto);
        }

        // POST: api/Chat/messages
        [HttpPost("messages")]
        public async Task<ActionResult<ChatMessageDto>> SendMessage(SendMessageDto dto)
        {
            var userId = GetCurrentUserId();

            var room = await _context.Set<ChatRoom>()
                .Include(r => r.Members)
                .FirstOrDefaultAsync(r => r.ChatRoomId == dto.ChatRoomId);

            if (room == null)
                return NotFound();

            if (!room.Members.Any(m => m.UserId == userId))
                return Forbid();

            var message = new ChatMessage
            {
                ChatRoomId = dto.ChatRoomId,
                SenderId = userId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Set<ChatMessage>().Add(message);

            // Update room's last message time
            room.LastMessageAt = message.SentAt;

            await _context.SaveChangesAsync();

            // Load sender info
            var sender = await _context.Users.FindAsync(userId);

            var messageDto = new ChatMessageDto
            {
                ChatMessageId = message.ChatMessageId,
                ChatRoomId = message.ChatRoomId,
                SenderId = message.SenderId,
                SenderName = sender?.UserName ?? "Unknown",
                Content = message.Content,
                SentAt = message.SentAt,
                IsEdited = false,
                IsOwnMessage = true
            };

            return Ok(messageDto);
        }

        // POST: api/Chat/rooms/{id}/mark-read
        [HttpPost("rooms/{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();

            var member = await _context.Set<ChatRoomMember>()
                .FirstOrDefaultAsync(m => m.ChatRoomId == id && m.UserId == userId);

            if (member == null)
                return NotFound();

            member.LastReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Chat/campaigns/{campaignId}/members
        [HttpGet("campaigns/{campaignId}/members")]
        public async Task<ActionResult<IEnumerable<object>>> GetCampaignMembers(int campaignId)
        {
            var userId = GetCurrentUserId();

            // Verify user is a member of the campaign
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);

            if (!isMember)
                return Forbid();

            var members = await _context.CampaignMembers
                .Where(cm => cm.CampaignId == campaignId)
                .Include(cm => cm.User)
                .Select(cm => new
                {
                    UserId = cm.UserId,
                    Username = cm.User.UserName,
                    Role = cm.Role
                })
                .ToListAsync();

            return Ok(members);
        }
    }
}