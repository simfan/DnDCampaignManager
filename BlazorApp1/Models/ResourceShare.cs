using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class ResourceShare
    {
        public int ResourceShareId { get; set; }

        public int ResourceId { get; set; }
        public string? UserId { get; set; }
        public int? CharacterId { get; set; }
        public ShareType ShareType { get; set; }

        // Navigation Properties
        public Resource? Resource { get; set; }
        public ApplicationUser? User { get; set; }
        public Character? Character { get; set; }

        // Metadata
        public DateTime SharedDate { get; set; } = DateTime.UtcNow;
        public string SharedById { get; set; } = string.Empty;
        public ApplicationUser? SharedBy { get; set; }

    }
    public enum ShareType
    {
        EntireCampaign = 1,
        SpecificUsers = 2,
        SpecificCharacters = 3
    }
}
