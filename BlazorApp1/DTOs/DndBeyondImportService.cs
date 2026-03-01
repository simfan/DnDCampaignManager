using BlazorApp1.Models;
using System.Text.Json;

namespace BlazorApp1.DTOs
{
    /// <summary>
    /// Maps a D&D Beyond character JSON payload to the app's CreateCharacterDto.
    /// </summary>
    public class DndBeyondImportService
    {
        private readonly HttpClient _httpClient;

        public DndBeyondImportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // DDB stat ID → ability score abbreviation
        private static readonly Dictionary<int, string> StatIdMap = new()
        {
            { 1, "STR" }, { 2, "DEX" }, { 3, "CON" },
            { 4, "INT" }, { 5, "WIS" }, { 6, "CHA" }
        };

        // DDB skill subType (from modifier) → Skill name in our app
        private static readonly Dictionary<string, string> SkillSubTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "acrobatics",       "Acrobatics" },
            { "animal-handling",  "Animal Handling" },
            { "arcana",           "Arcana" },
            { "athletics",        "Athletics" },
            { "deception",        "Deception" },
            { "history",          "History" },
            { "insight",          "Insight" },
            { "intimidation",     "Intimidation" },
            { "investigation",    "Investigation" },
            { "medicine",         "Medicine" },
            { "nature",           "Nature" },
            { "perception",       "Perception" },
            { "performance",      "Performance" },
            { "persuasion",       "Persuasion" },
            { "religion",         "Religion" },
            { "sleight-of-hand",  "Sleight of Hand" },
            { "stealth",          "Stealth" },
            { "survival",         "Survival" }
        };

        /// <summary>
        /// Deserialises the raw DDB JSON string and maps it to a CreateCharacterDto.
        /// </summary>
        public CreateCharacterDto Map(string ddbJson, int campaignId)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<DndBeyondCharacterData>(ddbJson, options)
               ?? throw new InvalidOperationException("Failed to deserialise D&D Beyond JSON.");

            /*var root = JsonSerializer.Deserialize<DndBeyondCharacterRoot>(ddbJson, options)
                       ?? throw new InvalidOperationException("Failed to deserialise D&D Beyond JSON.");

           /* var data = root.Data
                       ?? throw new InvalidOperationException("D&D Beyond JSON contained no 'data' object.");
           */
            return new CreateCharacterDto
            {
                CampaignId = campaignId,
                Name = data.Name,
                Race = ResolveRace(data.Race),
                Classes = ResolveClasses(data.Classes),
                Strength = ResolveAbilityScore(data, 1),
                Dexterity = ResolveAbilityScore(data, 2),
                Constitution = ResolveAbilityScore(data, 3),
                Intelligence = ResolveAbilityScore(data, 4),
                Wisdom = ResolveAbilityScore(data, 5),
                Charisma = ResolveAbilityScore(data, 6),
                MaxHitPoints = ResolveMaxHp(data),
                CurrentHitPoints = ResolveCurrentHp(data),
                ArmorClass = ResolveArmorClass(data),
                Skills = ResolveSkills(data)
            };
        }

        // ── Private helpers ──────────────────────────────────────────────────

        private static string ResolveRace(DndBeyondRace? race)
            => race?.FullName ?? race?.BaseName ?? "Unknown";

        private static List<CharacterClassDto> ResolveClasses(List<DndBeyondClass> classes)
            => classes
                .Where(c => c.Definition != null)
                .Select(c => new CharacterClassDto
                {
                    Name = c.Definition!.Name,
                    Level = c.Level
                })
                .ToList();

        /// <summary>
        /// DDB stores base score, racial/feat bonuses as modifiers, and optional overrides.
        /// Priority: override > base + bonuses.
        /// </summary>
        private static int ResolveAbilityScore(DndBeyondCharacterData data, int statId)
        {
            // Check for a hard override first
            var overrideVal = data.OverrideStats.FirstOrDefault(s => s.Id == statId)?.Value;
            if (overrideVal.HasValue) return overrideVal.Value;

            int baseVal = data.Stats.FirstOrDefault(s => s.Id == statId)?.Value ?? 10;
            int bonusVal = data.BonusStats.FirstOrDefault(s => s.Id == statId)?.Value ?? 0;

            // Also sum any racial/feat modifier bonuses for this ability
            string abilitySubType = StatIdMap[statId].ToLower(); // e.g. "str"
            int modifierBonus = AllModifiers(data)
                .Where(m => m.Type == "bonus"
                         && m.SubType.EndsWith(abilitySubType, StringComparison.OrdinalIgnoreCase))
                .Sum(m => m.Value ?? 0);

            return baseVal + bonusVal + modifierBonus;
        }

        /* private static int ResolveMaxHp(DndBeyondCharacterData data)
             => data.HitPointInfo?.MaxHp ?? 1;*/
        private static int ResolveMaxHp(DndBeyondCharacterData data)
        {
            if (data.OverrideHitPoints.HasValue) return data.OverrideHitPoints.Value;
            return data.BaseHitPoints + (data.BonusHitPoints ?? 0);
        }

        private static int ResolveCurrentHp(DndBeyondCharacterData data)
        {
            int max = ResolveMaxHp(data);
            //int removed = data.HitPointInfo?.RemovedHp ?? 0;
            return Math.Max(0, max - data.RemovedHitPoints);
        }

        private static int? GetIntValue(DndBeyondCharacterValue v)
        {
            return v.Value.ValueKind == JsonValueKind.Number
                ? v.Value.GetInt32()
                : null;
        }

        private static int ResolveArmorClass(DndBeyondCharacterData data)
        {
            // Check for an AC override stored in characterValues (typeId 6)
            var acOverride = data.CharacterValues.FirstOrDefault(v => v.TypeId == 6);
            //if (acOverride.HasValue) return acOverride.Value;
            if (acOverride != null)
            {
                var val = GetIntValue(acOverride);
                if (val.HasValue) return val.Value;
            }
            // Fallback: base 10 + DEX modifier + any equipped armour bonus
            int dexMod = (ResolveAbilityScore(data, 2) - 10) / 2;

            var equippedArmour = data.Inventory
                .Where(i => i.Equipped && i.Definition?.ArmorTypeId != null && i.Definition.ArmorClass.HasValue)
                .OrderByDescending(i => i.Definition!.ArmorClass)
                .FirstOrDefault();

            if (equippedArmour?.Definition?.ArmorClass != null)
            {
                // Heavy armour (typeId 4) doesn't add DEX; medium (3) caps at +2; light (2) adds full DEX
                int armourTypeId = equippedArmour.Definition.ArmorTypeId ?? 0;
                int dexContrib = armourTypeId switch
                {
                    4 => 0,
                    3 => Math.Min(dexMod, 2),
                    _ => dexMod
                };
                return equippedArmour.Definition.ArmorClass.Value + dexContrib;
            }

            return 10 + dexMod;
        }

        private static List<Skill> ResolveSkills(DndBeyondCharacterData data)
        {
            var allModifiers = AllModifiers(data).ToList();

            // Build sets of proficient / expertise skill subTypes
            var proficientSubTypes = allModifiers
                .Where(m => m.Type == "proficiency" && m.SubType.StartsWith("skill:"))
                .Select(m => m.SubType["skill:".Length..])
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var expertiseSubTypes = allModifiers
                .Where(m => m.Type == "expertise" && m.SubType.StartsWith("skill:"))
                .Select(m => m.SubType["skill:".Length..])
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var skills = SkillDefinitions.GetAllSkills();
            foreach (var skill in skills)
            {
                // Find the DDB subType that matches this skill name
                var ddbKey = SkillSubTypeMap
                    .FirstOrDefault(kv => kv.Value == skill.Name).Key;

                if (ddbKey != null)
                {
                    skill.IsProficient = proficientSubTypes.Contains(ddbKey);
                    skill.HasExpertise = expertiseSubTypes.Contains(ddbKey);
                }
            }

            return skills;
        }
        public async Task<CreateCharacterDto> ImportFromDndBeyondAsync(long characterId, int campaignId)
        {
            var url = $"https://www.dndbeyond.com/character/{characterId}/json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("D&D Beyond returned an error. Make sure the character is set to public sharing.");

            var json = await response.Content.ReadAsStringAsync();
            return Map(json, campaignId);
        }

        public async Task<CreateCharacterDto> ImportByIdAsync(long characterId, int campaignId)
        {
            var url = $"https://www.dndbeyond.com/character/{characterId}/json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("D&D Beyond returned an error. Make sure the character is set to public sharing.");

            var json = await response.Content.ReadAsStringAsync();
            return Map(json, campaignId);
        }

        // Used when pasting raw JSON directly
        public CreateCharacterDto ImportFromJson(string rawJson, int campaignId)
        {
            return Map(rawJson, campaignId);
        }
        private static IEnumerable<DndBeyondModifier> AllModifiers(DndBeyondCharacterData data)
        {
            if (data.Modifiers == null) return Enumerable.Empty<DndBeyondModifier>();

            return data.Modifiers.Class
                .Concat(data.Modifiers.Race)
                .Concat(data.Modifiers.Background)
                .Concat(data.Modifiers.Feat)
                .Concat(data.Modifiers.Item);
        }
    }
}