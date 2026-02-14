using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class PlayerEvent
    {
        public int PlayerEventId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int? CampaignId { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        public Campaign? Campaign { get; set; }
        public ApplicationUser? User { get; set; }

        // Metadata
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
    }
}