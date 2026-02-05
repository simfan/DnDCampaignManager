using BlazorApp1.Models;
using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // Add your custom properties
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Campaign> CreatedCampaigns { get; set; } = new List<Campaign>();
        public List<CampaignMember> CampaignMemberships { get; set; } = new List<CampaignMember>();

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
    }

}
