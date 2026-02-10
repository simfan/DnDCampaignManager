using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class Campaign
    {
        public int CampaignId { get; set; }

        // Basic Info
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // DM/Owner
        public string CreatedById { get; set; } // The DM

        // Navigation Properties
        public ApplicationUser? CreatedBy { get; set; } // The DM
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<CampaignMember> Members { get; set; } = new List<CampaignMember>();
        public Journal? Journal { get; set; }

        // Metadata
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
