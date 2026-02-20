using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public enum ChatRoomType
    {
        CampaignWide,  // All campaign members
        Direct,        // One-on-one
        Group          // Custom group
    }

    public class ChatRoom
    {
        public int ChatRoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ChatRoomType Type { get; set; }
        public int? CampaignId { get; set; }  // Null for direct messages between users from different campaigns
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastMessageAt { get; set; }

        // Navigation properties
        public Campaign? Campaign { get; set; }
        public ApplicationUser CreatedBy { get; set; } = null!;
        public List<ChatRoomMember> Members { get; set; } = new();
        public List<ChatMessage> Messages { get; set; } = new();
    }
}