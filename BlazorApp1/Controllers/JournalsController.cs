using BlazorApp1.Data;
using BlazorApp1.Models;
using BlazorApp1.DTOs;
using BlazorApp1.Conversions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JournalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JournalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JournalDto>>> GetJournals()
        {
            var journals = await _context.Journals.ToListAsync();
            List<JournalDto>? journalDtos = new();
            foreach (var journal in journals)
            {
                var journalDto = JournalConversions.JournalToDto(journal);
                journalDtos.Add(journalDto);
            }
            return Ok(journalDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JournalDetailDto>> GetJournal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var journal = await _context.Journals
                .Include(j => j.Entries)
                .Include(j => j.Tags)
                .FirstOrDefaultAsync(j => j.JournalId == id);

            //If this is a character journal (1), make sure user has permission to read it
            if(journal == null)
            {
                return NotFound();
            }

            switch (journal.JournalTypeId)
            {
                case ((int)JournalType.Player):
                    if(journal.OwnerId != userId)
                    {
                        return Forbid();
                    }
                    break;
                case ((int)JournalType.Character):

                    var character = await _context.Characters.FirstOrDefaultAsync(c => c.CharacterId == Int32.Parse(journal.OwnerId));
                    var isDM = await _context.CampaignMembers
                        .AnyAsync(cm => cm.CampaignId == character.CampaignId
                        && cm.UserId == userId
                        && cm.Role == CampaignRole.DM);
                    if(character.PlayerId != userId && !isDM)
                    {
                        return Forbid();
                    }
                    break;
                case ((int)JournalType.Campaign):
                    var campaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.CampaignId == Int32.Parse(journal.OwnerId));
                    var isMember = await _context.CampaignMembers
                        .AnyAsync(cm => cm.CampaignId == campaign.CampaignId
                        && cm.UserId == userId);
                    if(!isMember)
                    {
                        return Forbid();
                    }
                    break;
            }

            var journalDto = JournalConversions.JournalToDetailDto(journal);
            return journalDto;
        }

        [HttpPost()]
        public async Task<ActionResult<JournalDto>> CreateJournal(CreateJournalDto createdJournal)
        {
            var journal = JournalConversions.CreateToJournal(createdJournal);
            _context.Journals.Add(journal);
            await _context.SaveChangesAsync();
            var journalDto = JournalConversions.JournalToDto(journal);
            return Ok(journalDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJournal(int id, UpdateJournalDto updatedJournal)
        {
            var currentJournal = await _context.Journals.FirstOrDefaultAsync(j => j.JournalId == id);
            currentJournal = JournalConversions.UpdateToJournal(updatedJournal, currentJournal.CreatedAt);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
    }
}

