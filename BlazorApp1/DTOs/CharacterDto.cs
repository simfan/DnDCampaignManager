namespace BlazorApp1.DTOs
{
    public class CharacterDto
    {
        public int CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public int ExperiencePoints { get; set; }
        public int TotalLevel { get; set; }

        public List<CharacterClassDto> Classes { get; set; } = new();

        // Ability Scores
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        // Derived Stats
        public int ArmorClass { get; set; }
        public int MaxHitPoints { get; set; }
        public int CurrentHitPoints { get; set; }

        // Foreign Keys
        public int CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    public class CreateCharacterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public int CampaignId { get; set; }

        public List<CharacterClassDto> Classes { get; set; } = new();

        // Ability Scores
        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Constitution { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Wisdom { get; set; } = 10;
        public int Charisma { get; set; } = 10;

        // Derived Stats
        public int ArmorClass { get; set; } = 10;
        public int MaxHitPoints { get; set; } = 10;
        public int CurrentHitPoints { get; set; } = 10;
    }

    public class UpdateCharacterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public int ExperiencePoints { get; set; }

        public List<CharacterClassDto> Classes { get; set; } = new();

        // Ability Scores
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        // Derived Stats
        public int ArmorClass { get; set; }
        public int MaxHitPoints { get; set; }
        public int CurrentHitPoints { get; set; }
    }

    public class CharacterClassDto
    {
        public int CharacterClassId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
    }
}