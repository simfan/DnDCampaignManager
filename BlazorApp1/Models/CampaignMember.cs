using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class CampaignMember
    {
        public int CampaignMemberId { get; set; }

        public int CampaignId { get; set; }
        public string UserId { get; set; }

        public CampaignRole Role { get; set; } = CampaignRole.Player;

        // Navigation Properties
        public Campaign? Campaign { get; set; }
        public ApplicationUser? User { get; set; }

        // Metadata
        public DateTime JoinedDate { get; set; } = DateTime.Now;
    }
    public enum CampaignRole
    {
        Player = 0,
        DM = 1
    }
}
