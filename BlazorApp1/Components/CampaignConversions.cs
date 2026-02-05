using BlazorApp1.DTOs;
using BlazorApp1.Models;

namespace BlazorApp1.Components
{
    public class CampaignConversions
    {
        public Campaign DtoToCampaign(CampaignDto dto)
        {
            return new Campaign()
            {
                CampaignId = dto.CampaignId,
                Name = dto.Name,
                Description = dto.Description,
                CreatedById = dto.CreatedById,
                CreatedDate = dto.CreatedDate,
                LastModifiedDate = dto.LastModifiedDate,
                IsActive = dto.IsActive,
            };
        }

        public CampaignDto CampaignToDto(Campaign campaign)
        {
            var dto = new CampaignDto
            {
                CampaignId = campaign.CampaignId,
                Name = campaign.Name,
                Description = campaign.Description,
                CreatedById = campaign.CreatedById,
                CreatedDate = campaign.CreatedDate,
                LastModifiedDate = campaign.LastModifiedDate,
                IsActive = campaign.IsActive,

            };
            dto.MemberCount = campaign.Members.Count();
            dto.CharacterCount = campaign.Characters.Count();
            return dto;
            //dto.CreatedByName = campaign.
        }

        public CampaignDetailDto CampaignToDetailDto(Campaign campaign)
        {
            var dto = new CampaignDetailDto 
            {
                CampaignId = campaign.CampaignId,
                Name = campaign.Name,
                Description = campaign.Description,
                CreatedById = campaign.CreatedById,
                CreatedDate = campaign.CreatedDate,
                LastModifiedDate = campaign.LastModifiedDate,
                IsActive= campaign.IsActive,
            };
            //dto.Characters 
            //dto.Members
            return dto;
        }
    }
}
