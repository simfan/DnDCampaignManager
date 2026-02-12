using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        // Basic Info
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceType Type { get; set; }

        // File Info
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        // Ownership
        public int CampaignId { get; set; }
        public string UploadedById { get; set; } = string.Empty;

        // Navigation Properties
        public Campaign? Campaign { get; set; }
        public ApplicationUser? UploadedBy { get; set; }
        public List<ResourceShare> Shares { get; set; } = new List<ResourceShare>();

        // Metadata
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public enum ResourceType
    {
        Map = 1,
        Image = 2,
        Document = 3,
        Audio = 4,
        Other = 99
    }
}
