namespace BlazorApp1.DTOs
{
    public class InventoryItemDto
    {
        public int InventoryItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Value { get; set; }
        public bool IsEquipped { get; set; }
        public int? CharacterId { get; set; }
        public string? CharacterName { get; set; }
        public int? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }

    public class CreateInventoryItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal? Weight { get; set; }
        public decimal? Value { get; set; }
        public bool IsEquipped { get; set; } = false;

        // Supply exactly one
        public int? CharacterId { get; set; }
        public int? CampaignId { get; set; }
    }

    public class UpdateInventoryItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Value { get; set; }
        public bool IsEquipped { get; set; }
    }

    public class TransferInventoryItemDto
    {
        public int InventoryItemId { get; set; }
        public int? ToCharacterId { get; set; }
        public int? ToCampaignId { get; set; }
        public int? QuantityToTransfer { get; set; } // null = all
    }
}