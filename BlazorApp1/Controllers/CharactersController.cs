using BlazorApp1.Conversions;
using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Models;
using BlazorApp1.Services;
using BlazorApp1.Services.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CharacterPdfService _pdfService;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public CharactersController(ApplicationDbContext context, CharacterPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // GET: api/Characters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterDto>>> GetCharacters()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get all characters belonging to the current user
            var characters = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                //.Where(c => c.PlayerId == userId)
                .ToListAsync();

            var characterDtos = characters.Select(c => MapToCharacterDto(c)).ToList();

            return Ok(characterDtos);
        }

        // GET: api/Characters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterDto>> GetCharacter(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var character = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(c => c.CharacterId == id);

            if (character == null)
            {
                return NotFound();
            }

            // Check if user owns this character OR is DM of the campaign
            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == character.CampaignId
                    && cm.UserId == userId
                    && cm.Role == CampaignRole.DM);

            if (character.PlayerId != userId && !isDM)
            {
                return Forbid();
            }

            var characterDto = MapToCharacterDto(character);

            return Ok(characterDto);
        }

        // GET: api/Characters/campaign/5
        [HttpGet("campaign/{campaignId}")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharactersByCampaign(int campaignId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user is a member of this campaign
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var characters = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Player)
                .Where(c => c.CampaignId == campaignId)
                .ToListAsync();

            return Ok(characters);
        }
        [HttpGet("User")]
        public async Task<ActionResult<IEnumerable<CharacterDto>>> GetCharactersForCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var characters = await GetCharactersByUser(userId);
            return characters;
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<CharacterDto>>> GetCharactersByUser(string id)
        {
            var characters = await _context.Characters
                .Where(c => c.PlayerId == id)
                .ToListAsync();
            var characterDtos = new List<CharacterDto>();
            /*foreach(var character in characters)
            {
                var characterDto = await CharacterConversions.ConvertToCharacterDto(character);
                characterDtos.Add(characterDto);
            }*/
            return characterDtos;
        }
        // POST: api/Characters
        [HttpPost]
        public async Task<ActionResult<CharacterDto>> CreateCharacter(CreateCharacterDto createdCharacter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verify user is a member of the campaign
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == createdCharacter.CampaignId && cm.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var character = new Character
            {
                Name = createdCharacter.Name,
                Race = createdCharacter.Race,
                CampaignId = createdCharacter.CampaignId,
                PlayerId = userId,
                Strength = createdCharacter.Strength,
                Dexterity = createdCharacter.Dexterity,
                Constitution = createdCharacter.Constitution,
                Intelligence = createdCharacter.Intelligence,
                Wisdom = createdCharacter.Wisdom,
                Charisma = createdCharacter.Charisma,
                ArmorClass = createdCharacter.ArmorClass,
                MaxHitPoints = createdCharacter.MaxHitPoints,
                CurrentHitPoints = createdCharacter.CurrentHitPoints,
                ExperiencePoints = 0,
                CreatedDate = DateTime.UtcNow,
                Classes = createdCharacter.Classes.Select(cc => new CharacterClass
                {
                    Name = cc.Name,
                    Level = cc.Level,
                    Subclass = cc.Subclass
                }).ToList()
            };
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            var characterDto = MapToCharacterDto(character!);

            return CreatedAtAction(nameof(GetCharacter), new { id = character.CharacterId }, characterDto);
        }

        // PUT: api/Characters/5
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<CharacterDto>>> UpdateCharacter(int id, UpdateCharacterDto character)
        {
            /*if (id != character.CharacterId)
            {
                return BadRequest();
            }*/

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingCharacter = await _context.Characters
                    .Include(c => c.Classes)
                    .FirstOrDefaultAsync(c => c.CharacterId == id);
            if (existingCharacter == null)
            {
                return NotFound();
            }

            // Check if user owns this character OR is DM
            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == existingCharacter.CampaignId
                    && cm.UserId == userId
                    && cm.Role == CampaignRole.DM);

            if (existingCharacter.PlayerId != userId && !isDM)
            {
                return Forbid();
            }

            // Update properties
            existingCharacter.Name = character.Name;
            existingCharacter.Race = character.Race;
            existingCharacter.ExperiencePoints = character.ExperiencePoints;
            existingCharacter.Strength = character.Strength;
            existingCharacter.Dexterity = character.Dexterity;
            existingCharacter.Constitution = character.Constitution;
            existingCharacter.Intelligence = character.Intelligence;
            existingCharacter.Wisdom = character.Wisdom;
            existingCharacter.Charisma = character.Charisma;
            existingCharacter.ArmorClass = character.ArmorClass;
            existingCharacter.MaxHitPoints = character.MaxHitPoints;
            existingCharacter.CurrentHitPoints = character.CurrentHitPoints;
            existingCharacter.LastUpdatedDate = DateTime.UtcNow;

            if(character.Skills?.Count > 0)
            {
                existingCharacter.SkillsJson = JsonSerializer.Serialize(character.Skills, _jsonOptions);
            }
            //existingCharacter.SkillsJson = JsonSerializer.Serialize(character.Skills ?? new List<Skill>());
            _context.CharacterClasses.RemoveRange(existingCharacter.Classes);
            foreach (var classDto in character.Classes)
            {
                var characterClass = new CharacterClass
                {
                    CharacterId = existingCharacter.CharacterId,
                    Name = classDto.Name,
                    Level = classDto.Level,
                    Subclass = classDto.Subclass
                };
                _context.CharacterClasses.Add(characterClass);
            }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharacterExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }

            return NoContent();
        }

        // DELETE: api/Characters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            // Only owner can delete
            if (character.PlayerId != userId)
            {
                return Forbid();
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET /api/characters/export/{id}/json
        [HttpGet("export/{id}/json")]
        public async Task<IActionResult> ExportJson(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(c => c.CharacterId == id);

            if (character == null) return NotFound();
            var characterDto = MapToCharacterDto(character);
            var json = JsonSerializer.Serialize(characterDto, new JsonSerializerOptions { WriteIndented = true });
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"{characterDto.Name.Replace(" ", "_")}_character.json");
        }

        // GET /api/characters/export/{id}/pdf
        [HttpGet("export/{id}/pdf")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(c => c.CharacterId == id);
            var characterDto = MapToCharacterDto(character);
            if (character == null) return NotFound();

            var pdf = _pdfService.GenerateCharacterSheet(characterDto);
            return File(pdf, "application/pdf", $"{characterDto.Name.Replace(" ", "_")}_character_sheet.pdf");
        }
        [HttpPost("import/dndbeyond")]
        public async Task<ActionResult<CharacterDto>> ImportFromDndBeyond(
    [FromBody] DndBeyondImportRequest request,
    [FromServices] DndBeyondImportService importService)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == request.CampaignId && cm.UserId == userId);

            if (!isMember)
                return Forbid();

            CreateCharacterDto createDto;
            try
            {
                if (request.DndBeyondCharacterId.HasValue)
                    createDto = await importService.ImportByIdAsync(request.DndBeyondCharacterId.Value, request.CampaignId);
                else if (!string.IsNullOrWhiteSpace(request.RawJson))
                    createDto = importService.ImportFromJson(request.RawJson, request.CampaignId);
                else
                    return BadRequest("Provide either 'dndBeyondCharacterId' or 'rawJson'.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to import character: {ex.Message}");
            }

            var character = new Character
            {
                Name = createDto.Name,
                Race = createDto.Race,
                CampaignId = createDto.CampaignId,
                PlayerId = userId,
                Strength = createDto.Strength,
                Dexterity = createDto.Dexterity,
                Constitution = createDto.Constitution,
                Intelligence = createDto.Intelligence,
                Wisdom = createDto.Wisdom,
                Charisma = createDto.Charisma,
                ArmorClass = createDto.ArmorClass,
                MaxHitPoints = createDto.MaxHitPoints,
                CurrentHitPoints = createDto.CurrentHitPoints,
                ExperiencePoints = 0,
                CreatedDate = DateTime.UtcNow,
                SkillsJson = JsonSerializer.Serialize(createDto.Skills),
                Classes = createDto.Classes.Select(cc => new CharacterClass
                {
                    Name = cc.Name,
                    Level = cc.Level
                }).ToList()
            };

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCharacter),
                new { id = character.CharacterId },
                MapToCharacterDto(character));
        }
        // POST: api/Characters/import/dndbeyond
        /*[HttpPost("import/dndbeyond")]
        public async Task<ActionResult<CharacterDto>> ImportFromDndBeyond(
            [FromBody] DndBeyondImportRequest request,
            [FromServices] DndBeyondImportService importService)/*,
            [FromServices] IHttpClientFactory httpClientFactory)*/
        /*{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verify the user is a member of the target campaign
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == request.CampaignId && cm.UserId == userId);

            if (!isMember)
                return Forbid();

            // ── Resolve the raw DDB JSON ────────────────────────────────────────────
            string ddbJson;

            if (!string.IsNullOrWhiteSpace(request.RawJson))
            {
                // User pasted the JSON directly
                ddbJson = request.RawJson;
            }
            else if (request.DndBeyondCharacterId.HasValue)
            {
                // Fetch from the unofficial DDB endpoint
                var httpClient = httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (compatible; DnDManagerImporter/1.0)");

                var url = $"https://www.dndbeyond.com/character/{request.DndBeyondCharacterId}/json";

                HttpResponseMessage response;
                try
                {
                    response = await httpClient.GetAsync(url);
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(502, $"Failed to reach D&D Beyond: {ex.Message}");
                }

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode,
                        "D&D Beyond returned an error. Make sure the character is set to public sharing.");
                }

                ddbJson = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return BadRequest("Provide either 'dndBeyondCharacterId' or 'rawJson'.");
            }

            // ── Map to CreateCharacterDto ───────────────────────────────────────────
            CreateCharacterDto createDto;
            try
            {
                createDto = importService.Map(ddbJson, request.CampaignId);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to parse D&D Beyond character data: {ex.Message}");
            }

            // ── Persist (reuses existing create logic) ─────────────────────────────
            var character = new Character
            {
                Name = createDto.Name,
                Race = createDto.Race,
                CampaignId = createDto.CampaignId,
                PlayerId = userId,
                Strength = createDto.Strength,
                Dexterity = createDto.Dexterity,
                Constitution = createDto.Constitution,
                Intelligence = createDto.Intelligence,
                Wisdom = createDto.Wisdom,
                Charisma = createDto.Charisma,
                ArmorClass = createDto.ArmorClass,
                MaxHitPoints = createDto.MaxHitPoints,
                CurrentHitPoints = createDto.CurrentHitPoints,
                ExperiencePoints = 0,
                CreatedDate = DateTime.UtcNow,
                SkillsJson = JsonSerializer.Serialize(createDto.Skills),
                Classes = createDto.Classes.Select(cc => new CharacterClass
                {
                    Name = cc.Name,
                    Level = cc.Level
                }).ToList()
            };

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCharacter),
                new { id = character.CharacterId },
                MapToCharacterDto(character));
        }*/


        private bool CharacterExists(int id)
        {
            return _context.Characters.Any(e => e.CharacterId == id);
        }

        private CharacterDto MapToCharacterDto(Character character)
        {
            List<Skill> skills;
            {
                try
                {
                    skills = !string.IsNullOrEmpty(character.SkillsJson)
                    ? JsonSerializer.Deserialize<List<Skill>>(character.SkillsJson, _jsonOptions) ?? SkillDefinitions.GetAllSkills()
                    : SkillDefinitions.GetAllSkills();
                }
                catch
                {
                    skills = SkillDefinitions.GetAllSkills();
                }
                return new CharacterDto
                {
                    CharacterId = character.CharacterId,
                    Name = character.Name,
                    Race = character.Race,
                    ExperiencePoints = character.ExperiencePoints,
                    TotalLevel = character.Classes.Sum(c => c.Level),
                    Classes = character.Classes.Select(cc => new CharacterClassDto
                    {
                        CharacterClassId = cc.CharacterClassId,
                        Name = cc.Name,
                        Level = cc.Level,
                        Subclass = cc.Subclass
                    }).ToList(),
                    Skills = skills,
                    Strength = character.Strength,
                    Dexterity = character.Dexterity,
                    Constitution = character.Constitution,
                    Intelligence = character.Intelligence,
                    Wisdom = character.Wisdom,
                    Charisma = character.Charisma,
                    ArmorClass = character.ArmorClass,
                    MaxHitPoints = character.MaxHitPoints,
                    CurrentHitPoints = character.CurrentHitPoints,
                    CampaignId = character.CampaignId,
                    CampaignName = character.Campaign?.Name ?? "Unknown",
                    PlayerId = character.PlayerId,
                    PlayerName = character.Player?.UserName ?? "Unknown",
                    CreatedDate = character.CreatedDate,
                    LastModifiedDate = character.LastUpdatedDate
                };
            }
        }
    }
}