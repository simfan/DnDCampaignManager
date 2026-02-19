using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class ChatRoomMember
    {
        public int ChatRoomMemberId { get; set; }
        public int ChatRoomId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastReadAt { get; set; }

        // Navigation properties
        public ChatRoom ChatRoom { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}