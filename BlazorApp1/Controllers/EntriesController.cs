using BlazorApp1.Data;
using BlazorApp1.Models;
using BlazorApp1.DTOs;
using BlazorApp1.Conversions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryDto>>> GetEntries()
        {
            var entries = await _context.Entries.ToListAsync();
            List<EntryDto>? entryDtos = new();
            foreach (var entry in entries)
            {
                var entryDto = EntryConversions.EntryToDto(entry);
                entryDtos.Add(entryDto);
            }
            return Ok(entryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntryDetailDto>> GetEntry(int id)
        {
            var entry = await _context.Entries
                .Include(e => e.Tags)
                .ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(e => e.EntryId == id);

            var entryDetailDto = EntryConversions.EntryToDetailDto(entry);
            return entryDetailDto;
        }

        [HttpPost()]
        public async Task<ActionResult<EntryDto>> CreateEntry(CreateEntryDto createdEntry)
        {
            try
            {
                var entry = EntryConversions.CreateToEntry(createdEntry);
                _context.Entries.Add(entry);
                await _context.SaveChangesAsync();
                var entryDto = EntryConversions.EntryToDto(entry);
                return Ok(entryDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex.Message);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntry(int id, UpdateEntryDto updatedEntry)
        {
            var currentEntry = await _context.Entries.FirstOrDefaultAsync(e => e.EntryId == id);
            currentEntry = EntryConversions.UpdateToEntry(updatedEntry, (DateTime)currentEntry.CreatedAt);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
