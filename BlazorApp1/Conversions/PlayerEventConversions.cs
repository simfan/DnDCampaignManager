using BlazorApp1.DTOs;
using BlazorApp1.Models;

namespace BlazorApp1.Conversions
{
    public static class PlayerEventConversions
    {
        public static PlayerEvent DtoToPlayerEvent(PlayerEventDto dto)
        {
            return new PlayerEvent()
            {
                PlayerEventId = dto.PlayerEventId,
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CampaignId = dto.CampaignId,
                UserId = dto.UserId,
                CreatedDate = dto.CreatedDate,
                LastModifiedDate = dto.LastModifiedDate
            };
        }

        public static PlayerEventDto PlayerEventToDto(PlayerEvent playerEvent)
        {
            return new PlayerEventDto()
            {
                PlayerEventId = playerEvent.PlayerEventId,
                Title = playerEvent.Title,
                Description = playerEvent.Description,
                EventDate = playerEvent.EventDate,
                StartTime = playerEvent.StartTime,
                EndTime = playerEvent.EndTime,
                CampaignId = playerEvent.CampaignId,
                UserId = playerEvent.UserId,
                CreatedDate = playerEvent.CreatedDate,
                LastModifiedDate = playerEvent.LastModifiedDate
            };
        }

        public static PlayerEvent CreateToPlayerEvent(CreatePlayerEventDto dto)
        {
            var currentDate = DateTime.Now;
            return new PlayerEvent()
            {
                PlayerEventId = 0,
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CampaignId = dto.CampaignId,
                CreatedDate = currentDate,
                LastModifiedDate = currentDate
            };
        }

        public static PlayerEvent UpdateToPlayerEvent(UpdatePlayerEventDto dto, DateTime createdDate)
        {
            
            return new PlayerEvent()
            {
                PlayerEventId = 0,
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CampaignId = dto.CampaignId,
                CreatedDate = createdDate,
                LastModifiedDate = DateTime.Now
            };
        }
    }
}
