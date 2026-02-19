using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public int ChatRoomId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAt { get; set; }

        // Navigation properties
        public ChatRoom ChatRoom { get; set; } = null!;
        public ApplicationUser Sender { get; set; } = null!;
    }
}




