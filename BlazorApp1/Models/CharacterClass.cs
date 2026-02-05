namespace BlazorApp1.Models
{
    public class CharacterClass
    {
        public int CharacterClassId { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 1;

        public Character? Character { get; set; }
    }
}
