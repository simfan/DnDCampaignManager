namespace BlazorApp1.Models
{
    public class CharacterClass
    {
        public int CharacterClassId { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 1;

        /// <summary>
        /// The subclass chosen for this class entry (e.g. "Champion" for Fighter).
        /// Null / empty means no subclass has been chosen yet.
        /// </summary>
        public string? Subclass { get; set; }
        public Character? Character { get; set; }
    }
}
