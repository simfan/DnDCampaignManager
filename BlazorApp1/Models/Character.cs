using BlazorApp1.Data;

namespace BlazorApp1.Models
{
    public class Character
    {
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public string Race { get; set; }
        public List<CharacterClass> Classes { get; set; } = new List<CharacterClass>();
        

        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Constitution { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Wisdom { get; set; } = 10;
        public int Charisma { get; set; }= 10;

        public int ArmorClass { get; set; } = 10;
        public int MaxHitPoints { get; set; } = 10;
        public int CurrentHitPoints { get; set; } = 10;
        public int ExperiencePoints { get; set; } = 0;

        public int CampaignId { get; set; }
        public string PlayerId { get; set; }

        public Campaign? Campaign { get; set; }
        public ApplicationUser? Player { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdatedDate { get; set; }
        public int TotalLevel => Classes.Sum(c => c.Level);
    }
}
