namespace BlazorApp1.DTOs
{
    public class ResourceDto
    {
        public int ResourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string UploadedById { get; set; } = string.Empty;
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<ResourceShareDto> Shares { get; set; } = new();
    }

    public class CreateResourceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CampaignId { get; set; }
        // File will be uploaded separately via IFormFile
    }

    public class UpdateResourceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ResourceShareDto
    {
        public int ResourceShareId { get; set; }
        public int ResourceId { get; set; }
        public string ShareType { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public int? CharacterId { get; set; }
        public string? CharacterName { get; set; }
        public DateTime SharedDate { get; set; }
        public string SharedById { get; set; } = string.Empty;
        public string SharedByName { get; set; } = string.Empty;
    }

    public class ShareResourceDto
    {
        public int ResourceId { get; set; }
        public string ShareType { get; set; } = string.Empty; // "EntireCampaign", "SpecificUsers", "SpecificCharacters"
        public List<string>? UserIds { get; set; }
        public List<int>? CharacterIds { get; set; }
    }

}
