namespace BlazorApp1.Helpers
{
    /// <summary>
    /// Provides the list of valid subclasses for each D&D 5e class.
    /// Keys are lower-cased class names for case-insensitive look-up.
    /// </summary>
    public static class ClassSubclasses
    {
        private static readonly Dictionary<string, List<string>> _subclasses =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Barbarian"] = new()
                {
                    "Path of the Berserker",
                    "Path of the Totem Warrior",
                    "Path of the Ancestral Guardian",
                    "Path of the Storm Herald",
                    "Path of the Zealot",
                    "Path of the Beast",
                    "Path of Wild Magic",
                    "Path of the Battlerager",
                    "Path of the Giant",
                },
                ["Bard"] = new()
                {
                    "College of Lore",
                    "College of Valor",
                    "College of Glamour",
                    "College of Swords",
                    "College of Whispers",
                    "College of Creation",
                    "College of Eloquence",
                    "College of Spirits",
                },
                ["Cleric"] = new()
                {
                    "Arcana Domain",
                    "Death Domain",
                    "Forge Domain",
                    "Grave Domain",
                    "Knowledge Domain",
                    "Life Domain",
                    "Light Domain",
                    "Nature Domain",
                    "Order Domain",
                    "Peace Domain",
                    "Tempest Domain",
                    "Trickery Domain",
                    "Twilight Domain",
                    "War Domain",
                },
                ["Druid"] = new()
                {
                    "Circle of the Land",
                    "Circle of the Moon",
                    "Circle of Dreams",
                    "Circle of the Shepherd",
                    "Circle of Spores",
                    "Circle of Stars",
                    "Circle of Wildfire",
                },
                ["Fighter"] = new()
                {
                    "Battle Master",
                    "Champion",
                    "Eldritch Knight",
                    "Arcane Archer",
                    "Cavalier",
                    "Echo Knight",
                    "Psi Warrior",
                    "Rune Knight",
                    "Samurai",
                    "Purple Dragon Knight",
                },
                ["Monk"] = new()
                {
                    "Way of the Open Hand",
                    "Way of Shadow",
                    "Way of the Four Elements",
                    "Way of the Drunken Master",
                    "Way of the Kensei",
                    "Way of the Sun Soul",
                    "Way of the Ascendant Dragon",
                    "Way of Mercy",
                    "Way of the Astral Self",
                },
                ["Paladin"] = new()
                {
                    "Oath of Devotion",
                    "Oath of the Ancients",
                    "Oath of Vengeance",
                    "Oath of Conquest",
                    "Oath of Redemption",
                    "Oath of Glory",
                    "Oath of the Watchers",
                    "Oath of the Crown",
                    "Oathbreaker",
                },
                ["Ranger"] = new()
                {
                    "Hunter",
                    "Beast Master",
                    "Fey Wanderer",
                    "Gloom Stalker",
                    "Horizon Walker",
                    "Monster Slayer",
                    "Swarmkeeper",
                    "Drakewarden",
                },
                ["Rogue"] = new()
                {
                    "Thief",
                    "Assassin",
                    "Arcane Trickster",
                    "Inquisitive",
                    "Mastermind",
                    "Scout",
                    "Soulknife",
                    "Swashbuckler",
                    "Phantom",
                },
                ["Sorcerer"] = new()
                {
                    "Draconic Bloodline",
                    "Wild Magic",
                    "Aberrant Mind",
                    "Clockwork Soul",
                    "Divine Soul",
                    "Shadow Magic",
                    "Storm Sorcery",
                    "Lunar Sorcery",
                },
                ["Warlock"] = new()
                {
                    "The Archfey",
                    "The Fiend",
                    "The Great Old One",
                    "The Celestial",
                    "The Fathomless",
                    "The Genie",
                    "The Hexblade",
                    "The Undead",
                    "The Undying",
                },
                ["Wizard"] = new()
                {
                    "School of Abjuration",
                    "School of Conjuration",
                    "School of Divination",
                    "School of Enchantment",
                    "School of Evocation",
                    "School of Illusion",
                    "School of Necromancy",
                    "School of Transmutation",
                    "Bladesinging",
                    "Chronurgy Magic",
                    "Graviturgy Magic",
                    "Order of Scribes",
                    "War Magic",
                },
                ["Artificer"] = new()
                {
                    "Alchemist",
                    "Armorer",
                    "Artillerist",
                    "Battle Smith",
                },
            };

        /// <summary>
        /// Returns the subclasses available for the given class name.
        /// Returns an empty list if the class name is unrecognised.
        /// </summary>
        public static List<string> GetSubclasses(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                return new List<string>();

            return _subclasses.TryGetValue(className.Trim(), out var list)
                ? list
                : new List<string>();
        }

        /// <summary>
        /// All recognised class names (sorted alphabetically).
        /// </summary>
        public static IReadOnlyList<string> AllClasses =>
            _subclasses.Keys.OrderBy(k => k).ToList();
    }
}