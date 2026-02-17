using BlazorApp1.Conversions;
using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Helpers;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Security.Claims;
using BlazorApp1.Helpers;
namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerEventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerEventDto>>> GetPlayerEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var events = await _context.PlayerEvents
                .Include(e => e.Campaign)
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            var eventDtos = events.Select(e => new PlayerEventDto
            {
                PlayerEventId = e.PlayerEventId,
                Title = e.Title,
                Description = e.Description,
                EventDate = e.EventDate,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CampaignId = e.CampaignId,
                CampaignName = e.Campaign?.Name ?? string.Empty,
                UserId = e.UserId,
                CreatedDate = e.CreatedDate,
                LastModifiedDate = e.LastModifiedDate
            }).ToList();

            return Ok(eventDtos);
        }

        // GET: api/PlayerEvents/month/2024/1
        [HttpGet("month/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<PlayerEventDto>>> GetEventsByMonth(int year, int month)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var events = await _context.PlayerEvents
                .Include(e => e.Campaign)
                .Where(e => e.UserId == userId && e.EventDate >= startDate && e.EventDate < endDate)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            var eventDtos = events.Select(e => new PlayerEventDto
            {
                PlayerEventId = e.PlayerEventId,
                Title = e.Title,
                Description = e.Description,
                EventDate = e.EventDate,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CampaignId = e.CampaignId,
                CampaignName = e.Campaign?.Name ?? string.Empty,
                UserId = e.UserId,
                CreatedDate = e.CreatedDate,
                LastModifiedDate = e.LastModifiedDate
            }).ToList();

            return Ok(eventDtos);
        }

        // GET: api/PlayerEvents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerEventDto>> GetPlayerEvent(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playerEvent = await _context.PlayerEvents
                .Include(e => e.Campaign)
                .FirstOrDefaultAsync(e => e.PlayerEventId == id);

            if (playerEvent == null)
            {
                return NotFound();
            }

            if (playerEvent.UserId != userId)
            {
                return Forbid();
            }

            var eventDto = new PlayerEventDto
            {
                PlayerEventId = playerEvent.PlayerEventId,
                Title = playerEvent.Title,
                Description = playerEvent.Description,
                EventDate = playerEvent.EventDate,
                StartTime = playerEvent.StartTime,
                EndTime = playerEvent.EndTime,
                CampaignId = playerEvent.CampaignId,
                CampaignName = playerEvent.Campaign?.Name ?? string.Empty,
                UserId = playerEvent.UserId,
                CreatedDate = playerEvent.CreatedDate,
                LastModifiedDate = playerEvent.LastModifiedDate
            };

            return Ok(eventDto);
        }

        //GET: api/PlayerEvents/{id}/export
        [HttpGet("{id}/export")]
        public async Task<IActionResult> ExportEvent(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playerEvent = await _context.PlayerEvents
                .Include(e => e.Campaign)
                .FirstOrDefaultAsync(e => e.PlayerEventId == id);

            if (playerEvent == null)
            {
                return NotFound();
            }

            if(playerEvent.UserId != userId)
            {
                return Forbid();
            }

            var eventDto = PlayerEventConversions.PlayerEventToDto(playerEvent);

            var icsContent = IcsFileGenerator.GenerateIcsFile(eventDto);
            var fileName = $"{SanitizeFileName(playerEvent.Title)}_{playerEvent.EventDate:yyyy-MM-dd}.ics";
            return File(
       System.Text.Encoding.UTF8.GetBytes(icsContent),
       "text/calendar",
       fileName
   );
        }
        //GET: api/PlayerEvents/{id}/export/all
        [HttpGet("{id}/export/all")]
        public async Task<IActionResult> ExportAllEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var events = await _context.PlayerEvents
                .Include(e => e.Campaign)
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            if (!events.Any())
            {
                return NotFound("No events found to export.");
            }
            List<PlayerEventDto> eventDtos = new();
            foreach (var thisEvent in events)
            {
                var eventDto = PlayerEventConversions.PlayerEventToDto(thisEvent);
                eventDtos.Add(eventDto);
            }

            var icsContent = IcsFileGenerator.GenerateIcsFile(eventDtos);
            var fileName = $"DnD_Events_{DateTime.Now:yyyy-MM-dd}.ics";

            return File(
                System.Text.Encoding.UTF8.GetBytes(icsContent),
                "text/calendar",
                fileName
            );
        }
            //GET: api/PlayerEvents/{id}/export/upcoming
            [HttpGet("{id}/export/upcoming")]
        public async Task<IActionResult> ExportUpcomingEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var events = await _context.PlayerEvents
                .Include(e => e.Campaign)
                .Where(e => e.UserId == userId && e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            if (!events.Any())
            {
                return NotFound("No upcoming events found to export.");
            }
            List<PlayerEventDto> eventDtos = new();
            foreach (var thisEvent in events)
            {
                var eventDto = PlayerEventConversions.PlayerEventToDto(thisEvent);
                eventDtos.Add(eventDto);
            }

            var icsContent = IcsFileGenerator.GenerateIcsFile(eventDtos);
            var fileName = $"DnD_Events_{DateTime.Now:yyyy-MM-dd}.ics";

            return File(
                System.Text.Encoding.UTF8.GetBytes(icsContent),
                "text/calendar",
                fileName
            );
        }
        // POST: api/PlayerEvents
        [HttpPost]
        public async Task<ActionResult<PlayerEventDto>> CreatePlayerEvent(CreatePlayerEventDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If CampaignId is provided, verify user has access to that campaign
            if (createDto.CampaignId.HasValue)
            {
                var hasAccess = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == createDto.CampaignId.Value && cm.UserId == userId);

                if (!hasAccess)
                {
                    return Forbid();
                }
            }

            var playerEvent = new PlayerEvent
            {
                Title = createDto.Title,
                Description = createDto.Description,
                EventDate = createDto.EventDate,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                CampaignId = createDto.CampaignId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.PlayerEvents.Add(playerEvent);
            await _context.SaveChangesAsync();

            var campaign = createDto.CampaignId.HasValue
                ? await _context.Campaigns.FindAsync(createDto.CampaignId.Value)
                : null;

            var eventDto = new PlayerEventDto
            {
                PlayerEventId = playerEvent.PlayerEventId,
                Title = playerEvent.Title,
                Description = playerEvent.Description,
                EventDate = playerEvent.EventDate,
                StartTime = playerEvent.StartTime,
                EndTime = playerEvent.EndTime,
                CampaignId = playerEvent.CampaignId,
                CampaignName = campaign?.Name ?? string.Empty,
                UserId = playerEvent.UserId,
                CreatedDate = playerEvent.CreatedDate,
                LastModifiedDate = playerEvent.LastModifiedDate
            };

            return CreatedAtAction(nameof(GetPlayerEvent), new { id = playerEvent.PlayerEventId }, eventDto);
        }

        // PUT: api/PlayerEvents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayerEvent(int id, UpdatePlayerEventDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playerEvent = await _context.PlayerEvents.FindAsync(id);
            if (playerEvent == null)
            {
                return NotFound();
            }

            if (playerEvent.UserId != userId)
            {
                return Forbid();
            }

            // If CampaignId is being changed, verify access
            if (updateDto.CampaignId.HasValue && updateDto.CampaignId != playerEvent.CampaignId)
            {
                var hasAccess = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == updateDto.CampaignId.Value && cm.UserId == userId);

                if (!hasAccess)
                {
                    return Forbid();
                }
            }

            playerEvent.Title = updateDto.Title;
            playerEvent.Description = updateDto.Description;
            playerEvent.EventDate = updateDto.EventDate;
            playerEvent.StartTime = updateDto.StartTime;
            playerEvent.EndTime = updateDto.EndTime;
            playerEvent.CampaignId = updateDto.CampaignId;
            playerEvent.LastModifiedDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerEventExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/PlayerEvents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerEvent(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playerEvent = await _context.PlayerEvents.FindAsync(id);
            if (playerEvent == null)
            {
                return NotFound();
            }

            if (playerEvent.UserId != userId)
            {
                return Forbid();
            }

            _context.PlayerEvents.Remove(playerEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerEventExists(int id)
        {
            return _context.PlayerEvents.Any(e => e.PlayerEventId == id);
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
        }
    }
}