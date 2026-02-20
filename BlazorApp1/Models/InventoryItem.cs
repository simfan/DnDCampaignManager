namespace BlazorApp1.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }   // e.g. Weapon, Armor, Consumable, Misc
        public int Quantity { get; set; } = 1;
        public decimal? Weight { get; set; }    // lbs
        public decimal? Value { get; set; }     // gold pieces
        public bool IsEquipped { get; set; } = false;

        // Ownership - either character OR campaign (one must be set)
        public int? CharacterId { get; set; }
        public int? CampaignId { get; set; }

        // Navigation
        public Character? Character { get; set; }
        public Campaign? Campaign { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
    }
}