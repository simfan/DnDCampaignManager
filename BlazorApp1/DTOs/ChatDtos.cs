namespace BlazorApp1.DTOs
{
    public class ChatRoomDto
    {
        public int ChatRoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string? LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public List<string> MemberNames { get; set; } = new();
    }

    public class CreateChatRoomDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;  // "CampaignWide", "Direct", "Group"
        public int? CampaignId { get; set; }
        public List<string> MemberUserIds { get; set; } = new();
    }

    public class ChatMessageDto
    {
        public int ChatMessageId { get; set; }
        public int ChatRoomId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsOwnMessage { get; set; }
    }

    public class SendMessageDto
    {
        public int ChatRoomId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class MarkAsReadDto
    {
        public int ChatRoomId { get; set; }
    }
}