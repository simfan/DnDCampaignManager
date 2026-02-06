namespace BlazorApp1.Models
{
    public class DiceRoll
    {
        public string RollType { get; set; } = string.Empty; // "Skill Check", "Saving Throw", "Attack Roll", etc.
        public List<int> Results { get; set; } = new();
        public int Total { get; set; }
        public int Modifier { get; set; }
        public int FinalTotal { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string DiceNotation { get; set; } = string.Empty; // e.g., "2d20+5"
    }

    public enum DiceType
    {
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D20 = 20,
        D100 = 100
    }

    public enum RollType
    {
        SkillCheck,
        SavingThrow,
        AttackRoll,
        DamageRoll,
        Initiative,
        Custom
    }
}