using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BlazorApp1.Models
{
    public class Skill
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("abilityScore")]
        public string AbilityScore  { get; set;} = string.Empty;
        [JsonPropertyName("isProficient")]
        public bool IsProficient { get; set; } = false;
        [JsonPropertyName("hasExpertise")]
        public bool HasExpertise { get; set; } = false;
        
    }

    public static class SkillDefinitions
    { 
        public static List<Skill>GetAllSkills()
        {
            return new List<Skill>
            {
                new Skill { Name = "Acrobatics", AbilityScore = "DEX" },
                new Skill { Name = "Animal Handling", AbilityScore = "WIS" },
                new Skill { Name = "Arcana", AbilityScore = "INT" },
                new Skill { Name = "Athletics", AbilityScore = "STR" },
                new Skill { Name = "Deception", AbilityScore = "CHA" },
                new Skill { Name = "History", AbilityScore = "INT" },
                new Skill { Name = "Insight", AbilityScore = "WIS" },
                new Skill { Name = "Intimidation", AbilityScore = "CHA" },
                new Skill { Name = "Investigation", AbilityScore = "INT" },
                new Skill { Name = "Medicine", AbilityScore = "WIS" },
                new Skill { Name = "Nature", AbilityScore = "INT" },
                new Skill { Name = "Perception", AbilityScore = "WIS" },
                new Skill { Name = "Performance", AbilityScore = "CHA" },
                new Skill { Name = "Persuasion", AbilityScore = "CHA" },
                new Skill { Name = "Religion", AbilityScore = "INT" },
                new Skill { Name = "Sleight of Hand", AbilityScore = "DEX" },
                new Skill { Name = "Stealth", AbilityScore = "DEX" },
                new Skill { Name = "Survival", AbilityScore = "WIS" }
            };
        }
    }
}
