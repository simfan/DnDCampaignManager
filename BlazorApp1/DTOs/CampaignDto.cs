namespace BlazorApp1.DTOs
{
    public class CampaignDto
    {
        public int CampaignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty; // Username of DM
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
        public int CharacterCount { get; set; }
    }

    public class CreateCampaignDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateCampaignDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CampaignDetailDto
    {
        public int CampaignId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public JournalDetailDto? Journal { get; set; }
        public List<CampaignMemberDto> Members { get; set; } = new();
        public List<CharacterDto> Characters { get; set; } = new();
    }

    public class CampaignMemberDto
    {
        public int CampaignMemberId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "DM" or "Player"
        public DateTime JoinedDate { get; set; }
    }
}