using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp1.DTOs
{
    /// <summary>
    /// Represents the root object returned by the unofficial D&D Beyond character JSON endpoint:
    /// https://www.dndbeyond.com/character/{id}/json
    /// Only the fields we need for import are mapped here.
    /// </summary>
    public class DndBeyondCharacterRoot
    {
        [JsonPropertyName("data")]
        public DndBeyondCharacterData? Data { get; set; }
    }

    /*public class DndBeyondCharacterData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("race")]
        public DndBeyondRace? Race { get; set; }

        [JsonPropertyName("classes")]
        public List<DndBeyondClass> Classes { get; set; } = new();

        [JsonPropertyName("stats")]
        public List<DndBeyondStat> Stats { get; set; } = new();

        [JsonPropertyName("bonusStats")]
        public List<DndBeyondStat> BonusStats { get; set; } = new();

        [JsonPropertyName("overrideStats")]
        public List<DndBeyondStat> OverrideStats { get; set; } = new();

        [JsonPropertyName("hitPointInfo")]
        public DndBeyondHitPointInfo? HitPointInfo { get; set; }

        [JsonPropertyName("currentXp")]
        public int CurrentXp { get; set; }

        [JsonPropertyName("inventory")]
        public List<DndBeyondInventoryItem> Inventory { get; set; } = new();

        // Skill proficiencies come from modifiers
        [JsonPropertyName("modifiers")]
        public DndBeyondModifiers? Modifiers { get; set; }

        // Used for AC calculation
        [JsonPropertyName("characterValues")]
        public List<DndBeyondCharacterValue> CharacterValues { get; set; } = new();
    }*/
    public class DndBeyondCharacterData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("race")]
        public DndBeyondRace? Race { get; set; }

        [JsonPropertyName("classes")]
        public List<DndBeyondClass> Classes { get; set; } = new();

        [JsonPropertyName("stats")]
        public List<DndBeyondStat> Stats { get; set; } = new();

        [JsonPropertyName("bonusStats")]
        public List<DndBeyondStat> BonusStats { get; set; } = new();

        [JsonPropertyName("overrideStats")]
        public List<DndBeyondStat> OverrideStats { get; set; } = new();

        // HP fields are flat in the real JSON
        [JsonPropertyName("baseHitPoints")]
        public int BaseHitPoints { get; set; }

        [JsonPropertyName("bonusHitPoints")]
        public int? BonusHitPoints { get; set; }

        [JsonPropertyName("overrideHitPoints")]
        public int? OverrideHitPoints { get; set; }

        [JsonPropertyName("removedHitPoints")]
        public int RemovedHitPoints { get; set; }

        [JsonPropertyName("temporaryHitPoints")]
        public int TemporaryHitPoints { get; set; }

        [JsonPropertyName("inventory")]
        public List<DndBeyondInventoryItem> Inventory { get; set; } = new();

        [JsonPropertyName("modifiers")]
        public DndBeyondModifiers? Modifiers { get; set; }

        [JsonPropertyName("characterValues")]
        public List<DndBeyondCharacterValue> CharacterValues { get; set; } = new();
    }

    public class DndBeyondRace
    {
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("baseName")]
        public string? BaseName { get; set; }
    }

    public class DndBeyondClass
    {
        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("definition")]
        public DndBeyondClassDefinition? Definition { get; set; }
    }

    public class DndBeyondClassDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Stat IDs: 1=STR, 2=DEX, 3=CON, 4=INT, 5=WIS, 6=CHA
    /// </summary>
    public class DndBeyondStat
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }
    }

    public class DndBeyondHitPointInfo
    {
        [JsonPropertyName("maxHp")]
        public int MaxHp { get; set; }

        [JsonPropertyName("removedHp")]
        public int RemovedHp { get; set; }

        [JsonPropertyName("temporaryHp")]
        public int TemporaryHp { get; set; }
    }

    public class DndBeyondModifiers
    {
        [JsonPropertyName("class")]
        public List<DndBeyondModifier> Class { get; set; } = new();

        [JsonPropertyName("race")]
        public List<DndBeyondModifier> Race { get; set; } = new();

        [JsonPropertyName("background")]
        public List<DndBeyondModifier> Background { get; set; } = new();

        [JsonPropertyName("feat")]
        public List<DndBeyondModifier> Feat { get; set; } = new();

        [JsonPropertyName("item")]
        public List<DndBeyondModifier> Item { get; set; } = new();
    }

    public class DndBeyondModifier
    {
        /// <summary>
        /// e.g. "proficiency", "expertise", "half-proficiency", "set", "bonus"
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// e.g. "skill:athletics", "ability-score:strength", "armor-class"
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public int? Value { get; set; }
    }

    public class DndBeyondCharacterValue
    {
        /// <summary>
        /// TypeId 1 = current HP override, TypeId 3 = temp HP, TypeId 6 = AC override
        /// </summary>
        [JsonPropertyName("typeId")]
        public int TypeId { get; set; }

        [JsonPropertyName("value")]
        public JsonElement Value { get; set; }
    }

    public class DndBeyondInventoryItem
    {
        [JsonPropertyName("definition")]
        public DndBeyondItemDefinition? Definition { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("equipped")]
        public bool Equipped { get; set; }
    }

    public class DndBeyondItemDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("armorClass")]
        public int? ArmorClass { get; set; }

        [JsonPropertyName("armorTypeId")]
        public int? ArmorTypeId { get; set; }
    }

    /// <summary>
    /// Request body for the import endpoint — either supply a DDB character ID
    /// (we'll fetch it) or paste the raw JSON directly.
    /// </summary>
    public class DndBeyondImportRequest
    {
        /// <summary>
        /// The numeric D&D Beyond character ID from the URL.
        /// e.g. for https://www.dndbeyond.com/characters/12345678 supply 12345678.
        /// </summary>
        [JsonPropertyName("dndBeyondCharacterId")]
        public long? DndBeyondCharacterId { get; set; }

        /// <summary>
        /// Raw JSON string pasted directly (alternative to providing the ID).
        /// </summary>
        [JsonPropertyName("rawJson")]
        public string? RawJson { get; set; }

        /// <summary>
        /// The campaign to add the imported character to.
        /// </summary>
        [JsonPropertyName("campaignId")]
        public int CampaignId { get; set; }
    }
}