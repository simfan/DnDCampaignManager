using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Models;
using BlazorApp1.Services.Server;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BlazorApp1.Controllers
{
    [Route("api/plugin/characters")]
    [ApiController]
    [EnableCors("StreamDeckLocal")]
    public class CharacterPluginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CharacterPdfService _pdfService;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public CharacterPluginController(ApplicationDbContext context, CharacterPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // GET /api/plugin/characters
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var characters = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .ToListAsync();

            return Ok(characters.Select(MapToDto));
        }

        // GET /api/plugin/characters/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(c => c.CharacterId == id);

            if (character == null) return NotFound();
            return Ok(MapToDto(character));
        }

        // GET /api/plugin/characters/export/{id}/json
        [HttpGet("export/{id}/json")]
        public async Task<IActionResult> ExportJson(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(c => c.CharacterId == id);

            if (character == null) return NotFound();

            var dto = MapToDto(character);
            var json = System.Text.Json.JsonSerializer.Serialize(dto, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"{dto.Name.Replace(" ", "_")}_character.json");
        }

        // GET /api/plugin/characters/export/{id}/pdf
        [HttpGet("export/{id}/pdf")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var character = await _context.Characters
                .Include(c => c.Classes)
                .Include(c => c.Campaign)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(c => c.CharacterId == id);
            var characterDto = MapToDto(character);
            if (character == null) return NotFound();

            var pdf = _pdfService.GenerateCharacterSheet(characterDto);
            return File(pdf, "application/pdf", $"{characterDto.Name.Replace(" ", "_")}_character_sheet.pdf");
        }

        private CharacterDto MapToDto(Character character)
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
                        Level = cc.Level
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