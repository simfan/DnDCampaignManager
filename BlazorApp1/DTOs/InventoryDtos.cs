using BlazorApp1.Models;

namespace BlazorApp1.DTOs
{
    /// <summary>
    /// Single flat DTO for all item types. Subtype fields are nullable;
    /// only the fields relevant to the ItemType will be populated.
    /// </summary>
    public class InventoryItemDto
    {
        public int InventoryItemId { get; set; }
        public ItemType ItemType { get; set; }
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

        // ── Weapon ──────────────────────────────────────────────────────────
        public WeaponAttackType? AttackType { get; set; }
        public WeaponAbility? AbilityUsed { get; set; }
        public int? DamageDiceCount { get; set; }
        public int? DamageDiceSides { get; set; }
        public int? DamageBonus { get; set; }
        public DamageType? DamageType { get; set; }
        public int? VersatileDiceCount { get; set; }
        public int? VersatileDiceSides { get; set; }
        public int? ReachFt { get; set; }
        public int? RangeNormalFt { get; set; }
        public int? RangeLongFt { get; set; }
        public bool? IsTwoHanded { get; set; }
        public bool? IsLight { get; set; }
        public bool? IsThrown { get; set; }
        public string? Properties { get; set; }

        // ── Armor ───────────────────────────────────────────────────────────
        public ArmorType? ArmorType { get; set; }
        public int? BaseArmorClass { get; set; }
        public int? MaxDexBonus { get; set; }
        public int? StrengthRequirement { get; set; }
        public bool? StealthDisadvantage { get; set; }
        public int? ArmorClassBonus { get; set; }

        // ── Magic Item ──────────────────────────────────────────────────────
        public ItemRarity? Rarity { get; set; }
        public bool? RequiresAttunement { get; set; }
        public bool? IsAttuned { get; set; }
        public int? MaxCharges { get; set; }
        public int? CurrentCharges { get; set; }
        public string? Effect { get; set; }

        // ── Potion ──────────────────────────────────────────────────────────
        // (Effect and Duration — Effect reused from MagicItem field above)
        public string? Duration { get; set; }
        public int? HealDiceCount { get; set; }
        public int? HealDiceSides { get; set; }
        public int? HealBonus { get; set; }

        // ── Ammunition ──────────────────────────────────────────────────────
        public string? AmmunitionType { get; set; }
        public int? DamageBonusOverride { get; set; }

        // ── Tool ────────────────────────────────────────────────────────────
        public string? ToolType { get; set; }
        public bool? HasProficiency { get; set; }
        public string? AssociatedSkill { get; set; }

        // ── Mount ───────────────────────────────────────────────────────────
        public int? SpeedFt { get; set; }
        public string? CarryingCapacity { get; set; }
        public string? MountType { get; set; }
    }

    /// <summary>Used for both Create and Update — ItemType drives which fields are relevant.</summary>
    public class SaveInventoryItemDto
    {
        public ItemType ItemType { get; set; } = ItemType.Generic;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal? Weight { get; set; }
        public decimal? Value { get; set; }
        public bool IsEquipped { get; set; } = false;

        // Ownership — supply exactly one on create; both null on update
        public int? CharacterId { get; set; }
        public int? CampaignId { get; set; }

        // ── Weapon ──────────────────────────────────────────────────────────
        public WeaponAttackType? AttackType { get; set; }
        public WeaponAbility? AbilityUsed { get; set; }
        public int? DamageDiceCount { get; set; }
        public int? DamageDiceSides { get; set; }
        public int? DamageBonus { get; set; }
        public DamageType? DamageType { get; set; }
        public int? VersatileDiceCount { get; set; }
        public int? VersatileDiceSides { get; set; }
        public int? ReachFt { get; set; }
        public int? RangeNormalFt { get; set; }
        public int? RangeLongFt { get; set; }
        public bool? IsTwoHanded { get; set; }
        public bool? IsLight { get; set; }
        public bool? IsThrown { get; set; }
        public string? Properties { get; set; }

        // ── Armor ───────────────────────────────────────────────────────────
        public ArmorType? ArmorType { get; set; }
        public int? BaseArmorClass { get; set; }
        public int? MaxDexBonus { get; set; }
        public int? StrengthRequirement { get; set; }
        public bool? StealthDisadvantage { get; set; }
        public int? ArmorClassBonus { get; set; }

        // ── Magic Item ──────────────────────────────────────────────────────
        public ItemRarity? Rarity { get; set; }
        public bool? RequiresAttunement { get; set; }
        public bool? IsAttuned { get; set; }
        public int? MaxCharges { get; set; }
        public int? CurrentCharges { get; set; }
        public string? Effect { get; set; }

        // ── Potion ──────────────────────────────────────────────────────────
        public string? Duration { get; set; }
        public int? HealDiceCount { get; set; }
        public int? HealDiceSides { get; set; }
        public int? HealBonus { get; set; }

        // ── Ammunition ──────────────────────────────────────────────────────
        public string? AmmunitionType { get; set; }
        public int? DamageBonusOverride { get; set; }

        // ── Tool ────────────────────────────────────────────────────────────
        public string? ToolType { get; set; }
        public bool? HasProficiency { get; set; }
        public string? AssociatedSkill { get; set; }

        // ── Mount ───────────────────────────────────────────────────────────
        public int? SpeedFt { get; set; }
        public string? CarryingCapacity { get; set; }
        public string? MountType { get; set; }
    }

    public class TransferInventoryItemDto
    {
        public int InventoryItemId { get; set; }
        public int? ToCharacterId { get; set; }
        public int? ToCampaignId { get; set; }
        public int? QuantityToTransfer { get; set; }
    }
}