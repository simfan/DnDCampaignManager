namespace BlazorApp1.Models
{
    public enum ItemType
    {
        Generic,
        Weapon,
        Armor,
        MagicItem,
        Potion,
        Ammunition,
        Tool,
        Mount
    }
    public enum WeaponAttackType { Melee, Ranged, MeleeOrRanged }
    public enum WeaponAbility { STR, DEX, STRorDEX }  // STRorDEX = finesse
    public enum DamageType { Slashing, Piercing, Bludgeoning, Fire, Cold, Lightning, Thunder, Acid, Poison, Necrotic, Radiant, Psychic, Force }
    public enum ArmorType { Light, Medium, Heavy, Shield }
    public enum ItemRarity { Common, Uncommon, Rare, VeryRare, Legendary, Artifact }

    public class InventoryItem
    {
        public int InventoryItemId { get; set; }
        public ItemType ItemType { get; set; } = ItemType.Generic;

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
    // ── Weapon ───────────────────────────────────────────────────────────────

    public class WeaponItem : InventoryItem
    {
        public WeaponAttackType AttackType { get; set; } = WeaponAttackType.Melee;
        public WeaponAbility AbilityUsed { get; set; } = WeaponAbility.STR;

        // Normal damage
        public int DamageDiceCount { get; set; } = 1;
        public int DamageDiceSides { get; set; } = 6;   // 4,6,8,10,12
        public int DamageBonus { get; set; } = 0;
        public DamageType DamageType { get; set; } = DamageType.Slashing;

        // Versatile (two-handed) damage — null if not versatile
        public int? VersatileDiceCount { get; set; }
        public int? VersatileDiceSides { get; set; }

        // Reach / range
        public int ReachFt { get; set; } = 5;           // melee reach in feet
        public int? RangeNormalFt { get; set; }         // ranged normal range
        public int? RangeLongFt { get; set; }           // ranged long range

        // Flags
        public bool IsTwoHanded { get; set; } = false;
        public bool IsLight { get; set; } = false;       // can dual wield
        public bool IsThrown { get; set; } = false;
        public bool HasStealth { get; set; } = false;    // grants stealth adv
        public string? Properties { get; set; }          // free-text for anything else
    }

    // ── Armor ────────────────────────────────────────────────────────────────

    public class ArmorItem : InventoryItem
    {
        public ArmorType ArmorType { get; set; } = ArmorType.Light;
        public int BaseArmorClass { get; set; } = 10;
        public int? MaxDexBonus { get; set; }            // null = unlimited (light), 2 = medium, 0 = heavy
        public int? StrengthRequirement { get; set; }
        public bool StealthDisadvantage { get; set; } = false;
        public int ArmorClassBonus { get; set; } = 0;   // shields / magic bonuses
    }
    // ── Magic Item ───────────────────────────────────────────────────────────

    public class MagicItem : InventoryItem
    {
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public bool RequiresAttunement { get; set; } = false;
        public bool IsAttuned { get; set; } = false;
        public int? MaxCharges { get; set; }
        public int? CurrentCharges { get; set; }
        public string? Effect { get; set; }
    }

    // ── Potion ───────────────────────────────────────────────────────────────

    public class PotionItem : InventoryItem
    {
        public string? Effect { get; set; }
        public string? Duration { get; set; }           // e.g. "1 hour", "Until next short rest"
        // Healing dice, if any — e.g. 2d4+2
        public int? HealDiceCount { get; set; }
        public int? HealDiceSides { get; set; }
        public int? HealBonus { get; set; }
    }
    // ── Ammunition ───────────────────────────────────────────────────────────

    public class AmmunitionItem : InventoryItem
    {
        public string? AmmunitionType { get; set; }     // e.g. "Arrow", "Bolt", "Bullet", "Blowgun needle"
        public int? DamageBonusOverride { get; set; }   // magic ammo (+1 arrows etc.)
    }

    // ── Tool / Kit ───────────────────────────────────────────────────────────

    public class ToolItem : InventoryItem
    {
        public string? ToolType { get; set; }           // e.g. "Artisan's Tools", "Thieves' Tools"
        public bool HasProficiency { get; set; } = false;
        public string? AssociatedSkill { get; set; }
    }

    // ── Mount / Vehicle ──────────────────────────────────────────────────────

    public class MountItem : InventoryItem
    {
        public int? SpeedFt { get; set; }
        public string? CarryingCapacity { get; set; }
        public string? MountType { get; set; }          // e.g. "Horse", "Warhorse", "Griffon", "Rowboat"
    }


}