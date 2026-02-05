using BlazorApp1.Data;
using BlazorApp1.Models;
using BlazorApp1.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CampaignsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampaignDto>>> GetCampaigns()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var campaigns = await _context.CampaignMembers
                .Where(cm => cm.UserId == userId)
                .Include(cm => cm.Campaign)
                .ThenInclude(c => c.CreatedBy)
                .Select(cm => cm.Campaign)
                .ToListAsync();

            var campaignDtos = campaigns.Select(c => new CampaignDto
            {
                CampaignId = c.CampaignId,
                Name = c.Name,
                Description = c.Description,
                CreatedById = c.CreatedById,
                CreatedByName = c.CreatedBy?.UserName ?? "Unknown",
                CreatedDate = c.CreatedDate,
                LastModifiedDate = c.LastModifiedDate,
                IsActive = c.IsActive,
                MemberCount = c.Members.Count,
                CharacterCount = c.Characters.Count
            }).ToList();

            return Ok(campaignDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignDetailDto>> GetCampaign(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var campaign = await _context.Campaigns
                .Include(c => c.CreatedBy)
                .Include(c => c.Members)
                .Include(c => c.Characters)
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if(campaign == null)
            {
                return NotFound();
            }
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == id && cm.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }
            var campaignDto = new CampaignDetailDto
            {
                CampaignId = campaign.CampaignId,
                Name = campaign.Name,
                Description = campaign.Description,
                CreatedById = campaign.CreatedById,
                CreatedByName = campaign.CreatedBy?.UserName ?? "Unknown",
                CreatedDate = campaign.CreatedDate,
                LastModifiedDate = campaign.LastModifiedDate,
                IsActive = campaign.IsActive,
                Members = campaign.Members.Select(m => new CampaignMemberDto
                {
                    CampaignMemberId = m.CampaignMemberId,
                    UserId = m.UserId,
                    Username = m.User?.UserName ?? "Unknown",
                    Role = m.Role.ToString(),
                    JoinedDate = m.JoinedDate
                }).ToList(),
                Characters = campaign.Characters.Select(ch => new CharacterDto
                {
                    CharacterId = ch.CharacterId,
                    Name = ch.Name,
                    Race = ch.Race,
                    ExperiencePoints = ch.ExperiencePoints,
                    TotalLevel = ch.Classes.Sum(c => c.Level),
                    Classes = ch.Classes.Select(cc => new CharacterClassDto
                    {
                        CharacterClassId = cc.CharacterClassId,
                        Name = cc.Name,
                        Level = cc.Level
                    }).ToList(),
                    Strength = ch.Strength,
                    Dexterity = ch.Dexterity,
                    Constitution = ch.Constitution,
                    Intelligence = ch.Intelligence,
                    Wisdom = ch.Wisdom,
                    Charisma = ch.Charisma,
                    ArmorClass = ch.ArmorClass,
                    MaxHitPoints = ch.MaxHitPoints,
                    CurrentHitPoints = ch.CurrentHitPoints,
                    CampaignId = ch.CampaignId,
                    CampaignName = campaign.Name,
                    PlayerId = ch.PlayerId,
                    PlayerName = ch.Player?.UserName ?? "Unknown",
                    CreatedDate = ch.CreatedDate,
                    LastModifiedDate = ch.LastUpdatedDate
                }).ToList()
            };

            return Ok(campaignDto);
        }

        [HttpPost]
        public async Task<ActionResult<CampaignDto>> CreateCampaign(CreateCampaignDto createCampaignDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentDateTime = DateTime.UtcNow;
            var campaign = new Campaign
            {
                Name = createCampaignDto.Name,
                Description = createCampaignDto.Description,
                CreatedById = userId,
                CreatedDate = currentDateTime,
                LastModifiedDate = currentDateTime,
                IsActive = true
            };
            
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            // Add creator as a DM member
            var membership = new CampaignMember
            {
                CampaignId = campaign.CampaignId,
                UserId = userId,
                Role = CampaignRole.DM,
                JoinedDate = currentDateTime
            };

            _context.CampaignMembers.Add(membership);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            var campaignDto = new CampaignDetailDto
            {
                CampaignId = campaign.CampaignId,
                Name = campaign.Name,
                Description = campaign.Description,
                CreatedById = campaign.CreatedById,
                CreatedByName = user?.UserName ?? "Unknown",
                CreatedDate = campaign.CreatedDate,
                LastModifiedDate = campaign.LastModifiedDate,
                IsActive = campaign.IsActive,
                Members = new(),
                Characters = new()
            };

            return CreatedAtAction(nameof(GetCampaign), new { id = campaign.CampaignId }, campaignDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaign(int id, UpdateCampaignDto updatedCampaign)
        {



            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user is the DM
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }

            if (campaign.CreatedById != userId)
            {
                return Forbid();
            }

            campaign.Name = updatedCampaign.Name;
            campaign.Description = updatedCampaign.Description;
            campaign.IsActive = updatedCampaign.IsActive;
            campaign.LastModifiedDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Campaigns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }

            // Only DM can delete
            if (campaign.CreatedById != userId)
            {
                return Forbid();
            }

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CampaignExists(int id)
        {
            return _context.Campaigns.Any(e => e.CampaignId == id);
        }
    }
}
