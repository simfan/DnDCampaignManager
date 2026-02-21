using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();

        // ── GET: api/Inventory/character/5 ──────────────────────────────────
        [HttpGet("character/{characterId}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetCharacterInventory(int characterId)
        {
            var userId = GetUserId();
            var character = await _context.Characters.FindAsync(characterId);
            if (character == null) return NotFound();

            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == character.CampaignId && cm.UserId == userId);
            if (!isMember) return Forbid();

            var items = await _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .Where(i => i.CharacterId == characterId)
                .OrderBy(i => i.ItemType).ThenBy(i => i.Name)
                .ToListAsync();

            return Ok(items.Select(MapToDto));
        }

        // ── GET: api/Inventory/campaign/5 ───────────────────────────────────
        [HttpGet("campaign/{campaignId}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetCampaignInventory(int campaignId)
        {
            var userId = GetUserId();
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);
            if (!isMember) return Forbid();

            var items = await _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .Where(i => i.CampaignId == campaignId)
                .OrderBy(i => i.ItemType).ThenBy(i => i.Name)
                .ToListAsync();

            return Ok(items.Select(MapToDto));
        }

        // ── GET: api/Inventory/item/5 ────────────────────────────────────────
        [HttpGet("item/{id}")]
        public async Task<ActionResult<InventoryItemDto>> GetItem(int id)
        {
            var userId = GetUserId();
            var item = await LoadItem(id);
            if (item == null) return NotFound();
            if (!await CanAccess(item, userId)) return Forbid();
            return Ok(MapToDto(item));
        }

        // ── POST: api/Inventory ──────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult<InventoryItemDto>> CreateItem(SaveInventoryItemDto dto)
        {
            var userId = GetUserId();

            if (dto.CharacterId == null && dto.CampaignId == null)
                return BadRequest("Must specify CharacterId or CampaignId.");
            if (dto.CharacterId != null && dto.CampaignId != null)
                return BadRequest("Cannot specify both CharacterId and CampaignId.");

            if (!await CanAccessTarget(dto.CharacterId, dto.CampaignId, userId))
                return Forbid();

            var item = CreateSubclassInstance(dto);
            ApplyCommonFields(item, dto);
            item.CharacterId = dto.CharacterId;
            item.CampaignId = dto.CampaignId;
            item.CreatedDate = DateTime.UtcNow;
            item.LastUpdatedDate = DateTime.UtcNow;

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
            await _context.Entry(item).Reference(i => i.Character).LoadAsync();
            await _context.Entry(item).Reference(i => i.Campaign).LoadAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.InventoryItemId }, MapToDto(item));
        }

        // ── PUT: api/Inventory/item/5 ────────────────────────────────────────
        [HttpPut("item/{id}")]
        public async Task<ActionResult<InventoryItemDto>> UpdateItem(int id, SaveInventoryItemDto dto)
        {
            var userId = GetUserId();
            var item = await LoadItem(id);
            if (item == null) return NotFound();
            if (!await CanAccess(item, userId)) return Forbid();

            // If type changed, we need to replace the entity (TPH discriminator is immutable)
            if (item.ItemType != dto.ItemType)
            {
                _context.InventoryItems.Remove(item);
                var newItem = CreateSubclassInstance(dto);
                ApplyCommonFields(newItem, dto);
                newItem.CharacterId = item.CharacterId;
                newItem.CampaignId = item.CampaignId;
                newItem.CreatedDate = item.CreatedDate;
                newItem.LastUpdatedDate = DateTime.UtcNow;
                _context.InventoryItems.Add(newItem);
                await _context.SaveChangesAsync();
                await _context.Entry(newItem).Reference(i => i.Character).LoadAsync();
                await _context.Entry(newItem).Reference(i => i.Campaign).LoadAsync();
                return Ok(MapToDto(newItem));
            }

            ApplyCommonFields(item, dto);
            ApplySubtypeFields(item, dto);
            item.LastUpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(MapToDto(item));
        }

        // ── DELETE: api/Inventory/item/5 ─────────────────────────────────────
        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = GetUserId();
            var item = await LoadItem(id);
            if (item == null) return NotFound();
            if (!await CanAccess(item, userId)) return Forbid();
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ── POST: api/Inventory/transfer ─────────────────────────────────────
        [HttpPost("transfer")]
        public async Task<ActionResult<InventoryItemDto>> Transfer(TransferInventoryItemDto dto)
        {
            var userId = GetUserId();

            if (dto.ToCharacterId == null && dto.ToCampaignId == null)
                return BadRequest("Must specify a destination.");
            if (dto.ToCharacterId != null && dto.ToCampaignId != null)
                return BadRequest("Cannot specify both destinations.");

            var item = await LoadItem(dto.InventoryItemId);
            if (item == null) return NotFound();
            if (!await CanAccess(item, userId)) return Forbid();
            if (!await CanAccessTarget(dto.ToCharacterId, dto.ToCampaignId, userId)) return Forbid();

            int qty = dto.QuantityToTransfer ?? item.Quantity;
            if (qty <= 0 || qty > item.Quantity) return BadRequest("Invalid quantity.");

            if (qty < item.Quantity)
            {
                // Partial: clone at destination, reduce source
                item.Quantity -= qty;
                item.LastUpdatedDate = DateTime.UtcNow;

                var clone = CreateSubclassInstance(MapToSaveDto(item));
                ApplyCommonFields(clone, MapToSaveDto(item));
                ApplySubtypeFields(clone, MapToSaveDto(item));
                clone.Quantity = qty;
                clone.IsEquipped = false;
                clone.CharacterId = dto.ToCharacterId;
                clone.CampaignId = dto.ToCampaignId;
                clone.CreatedDate = DateTime.UtcNow;
                clone.LastUpdatedDate = DateTime.UtcNow;

                _context.InventoryItems.Add(clone);
                await _context.SaveChangesAsync();
                await _context.Entry(clone).Reference(i => i.Character).LoadAsync();
                await _context.Entry(clone).Reference(i => i.Campaign).LoadAsync();
                return Ok(MapToDto(clone));
            }
            else
            {
                // Full transfer
                item.CharacterId = dto.ToCharacterId;
                item.CampaignId = dto.ToCampaignId;
                item.IsEquipped = false;
                item.LastUpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await _context.Entry(item).Reference(i => i.Character).LoadAsync();
                await _context.Entry(item).Reference(i => i.Campaign).LoadAsync();
                return Ok(MapToDto(item));
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private Task<InventoryItem?> LoadItem(int id) =>
            _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);

        private async Task<bool> CanAccess(InventoryItem item, string userId)
        {
            int campaignId = item.Character?.CampaignId ?? item.CampaignId ?? 0;
            return await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);
        }

        private async Task<bool> CanAccessTarget(int? characterId, int? campaignId, string userId)
        {
            if (characterId.HasValue)
            {
                var ch = await _context.Characters.FindAsync(characterId.Value);
                if (ch == null) return false;
                return await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == ch.CampaignId && cm.UserId == userId);
            }
            return await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId!.Value && cm.UserId == userId);
        }

        private static InventoryItem CreateSubclassInstance(SaveInventoryItemDto dto) => dto.ItemType switch
        {
            ItemType.Weapon => new WeaponItem(),
            ItemType.Armor => new ArmorItem(),
            ItemType.MagicItem => new MagicItem(),
            ItemType.Potion => new PotionItem(),
            ItemType.Ammunition => new AmmunitionItem(),
            ItemType.Tool => new ToolItem(),
            ItemType.Mount => new MountItem(),
            _ => new InventoryItem()
        };

        private static void ApplyCommonFields(InventoryItem item, SaveInventoryItemDto dto)
        {
            item.ItemType = dto.ItemType;
            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Category = dto.Category;
            item.Quantity = dto.Quantity;
            item.Weight = dto.Weight;
            item.Value = dto.Value;
            item.IsEquipped = dto.IsEquipped;
        }

        private static void ApplySubtypeFields(InventoryItem item, SaveInventoryItemDto dto)
        {
            switch (item)
            {
                case WeaponItem w:
                    w.AttackType = dto.AttackType ?? WeaponAttackType.Melee;
                    w.AbilityUsed = dto.AbilityUsed ?? WeaponAbility.STR;
                    w.DamageDiceCount = dto.DamageDiceCount ?? 1;
                    w.DamageDiceSides = dto.DamageDiceSides ?? 6;
                    w.DamageBonus = dto.DamageBonus ?? 0;
                    w.DamageType = dto.DamageType ?? DamageType.Slashing;
                    w.VersatileDiceCount = dto.VersatileDiceCount;
                    w.VersatileDiceSides = dto.VersatileDiceSides;
                    w.ReachFt = dto.ReachFt ?? 5;
                    w.RangeNormalFt = dto.RangeNormalFt;
                    w.RangeLongFt = dto.RangeLongFt;
                    w.IsTwoHanded = dto.IsTwoHanded ?? false;
                    w.IsLight = dto.IsLight ?? false;
                    w.IsThrown = dto.IsThrown ?? false;
                    w.Properties = dto.Properties;
                    break;

                case ArmorItem a:
                    a.ArmorType = dto.ArmorType ?? ArmorType.Light;
                    a.BaseArmorClass = dto.BaseArmorClass ?? 10;
                    a.MaxDexBonus = dto.MaxDexBonus;
                    a.StrengthRequirement = dto.StrengthRequirement;
                    a.StealthDisadvantage = dto.StealthDisadvantage ?? false;
                    a.ArmorClassBonus = dto.ArmorClassBonus ?? 0;
                    break;

                case MagicItem m:
                    m.Rarity = dto.Rarity ?? ItemRarity.Common;
                    m.RequiresAttunement = dto.RequiresAttunement ?? false;
                    m.IsAttuned = dto.IsAttuned ?? false;
                    m.MaxCharges = dto.MaxCharges;
                    m.CurrentCharges = dto.CurrentCharges;
                    m.Effect = dto.Effect;
                    break;

                case PotionItem p:
                    p.Effect = dto.Effect;
                    p.Duration = dto.Duration;
                    p.HealDiceCount = dto.HealDiceCount;
                    p.HealDiceSides = dto.HealDiceSides;
                    p.HealBonus = dto.HealBonus;
                    break;

                case AmmunitionItem am:
                    am.AmmunitionType = dto.AmmunitionType;
                    am.DamageBonusOverride = dto.DamageBonusOverride;
                    break;

                case ToolItem t:
                    t.ToolType = dto.ToolType;
                    t.HasProficiency = dto.HasProficiency ?? false;
                    t.AssociatedSkill = dto.AssociatedSkill;
                    break;

                case MountItem mo:
                    mo.SpeedFt = dto.SpeedFt;
                    mo.CarryingCapacity = dto.CarryingCapacity;
                    mo.MountType = dto.MountType;
                    break;
            }
        }

        private static SaveInventoryItemDto MapToSaveDto(InventoryItem i)
        {
            var dto = new SaveInventoryItemDto
            {
                ItemType = i.ItemType,
                Name = i.Name,
                Description = i.Description,
                Category = i.Category,
                Quantity = i.Quantity,
                Weight = i.Weight,
                Value = i.Value,
                IsEquipped = i.IsEquipped,
            };

            switch (i)
            {
                case WeaponItem w:
                    dto.AttackType = w.AttackType; dto.AbilityUsed = w.AbilityUsed;
                    dto.DamageDiceCount = w.DamageDiceCount; dto.DamageDiceSides = w.DamageDiceSides;
                    dto.DamageBonus = w.DamageBonus; dto.DamageType = w.DamageType;
                    dto.VersatileDiceCount = w.VersatileDiceCount; dto.VersatileDiceSides = w.VersatileDiceSides;
                    dto.ReachFt = w.ReachFt; dto.RangeNormalFt = w.RangeNormalFt; dto.RangeLongFt = w.RangeLongFt;
                    dto.IsTwoHanded = w.IsTwoHanded; dto.IsLight = w.IsLight; dto.IsThrown = w.IsThrown;
                    dto.Properties = w.Properties;
                    break;
                case ArmorItem a:
                    dto.ArmorType = a.ArmorType; dto.BaseArmorClass = a.BaseArmorClass;
                    dto.MaxDexBonus = a.MaxDexBonus; dto.StrengthRequirement = a.StrengthRequirement;
                    dto.StealthDisadvantage = a.StealthDisadvantage; dto.ArmorClassBonus = a.ArmorClassBonus;
                    break;
                case MagicItem m:
                    dto.Rarity = m.Rarity; dto.RequiresAttunement = m.RequiresAttunement;
                    dto.IsAttuned = m.IsAttuned; dto.MaxCharges = m.MaxCharges;
                    dto.CurrentCharges = m.CurrentCharges; dto.Effect = m.Effect;
                    break;
                case PotionItem p:
                    dto.Effect = p.Effect; dto.Duration = p.Duration;
                    dto.HealDiceCount = p.HealDiceCount; dto.HealDiceSides = p.HealDiceSides; dto.HealBonus = p.HealBonus;
                    break;
                case AmmunitionItem am:
                    dto.AmmunitionType = am.AmmunitionType; dto.DamageBonusOverride = am.DamageBonusOverride;
                    break;
                case ToolItem t:
                    dto.ToolType = t.ToolType; dto.HasProficiency = t.HasProficiency; dto.AssociatedSkill = t.AssociatedSkill;
                    break;
                case MountItem mo:
                    dto.SpeedFt = mo.SpeedFt; dto.CarryingCapacity = mo.CarryingCapacity; dto.MountType = mo.MountType;
                    break;
            }
            return dto;
        }

        internal static InventoryItemDto MapToDto(InventoryItem i)
        {
            var dto = new InventoryItemDto
            {
                InventoryItemId = i.InventoryItemId,
                ItemType = i.ItemType,
                Name = i.Name,
                Description = i.Description,
                Category = i.Category,
                Quantity = i.Quantity,
                Weight = i.Weight,
                Value = i.Value,
                IsEquipped = i.IsEquipped,
                CharacterId = i.CharacterId,
                CharacterName = i.Character?.Name,
                CampaignId = i.CampaignId,
                CampaignName = i.Campaign?.Name,
                CreatedDate = i.CreatedDate,
                LastUpdatedDate = i.LastUpdatedDate
            };

            switch (i)
            {
                case WeaponItem w:
                    dto.AttackType = w.AttackType; dto.AbilityUsed = w.AbilityUsed;
                    dto.DamageDiceCount = w.DamageDiceCount; dto.DamageDiceSides = w.DamageDiceSides;
                    dto.DamageBonus = w.DamageBonus; dto.DamageType = w.DamageType;
                    dto.VersatileDiceCount = w.VersatileDiceCount; dto.VersatileDiceSides = w.VersatileDiceSides;
                    dto.ReachFt = w.ReachFt; dto.RangeNormalFt = w.RangeNormalFt; dto.RangeLongFt = w.RangeLongFt;
                    dto.IsTwoHanded = w.IsTwoHanded; dto.IsLight = w.IsLight; dto.IsThrown = w.IsThrown;
                    dto.Properties = w.Properties;
                    break;
                case ArmorItem a:
                    dto.ArmorType = a.ArmorType; dto.BaseArmorClass = a.BaseArmorClass;
                    dto.MaxDexBonus = a.MaxDexBonus; dto.StrengthRequirement = a.StrengthRequirement;
                    dto.StealthDisadvantage = a.StealthDisadvantage; dto.ArmorClassBonus = a.ArmorClassBonus;
                    break;
                case MagicItem m:
                    dto.Rarity = m.Rarity; dto.RequiresAttunement = m.RequiresAttunement;
                    dto.IsAttuned = m.IsAttuned; dto.MaxCharges = m.MaxCharges;
                    dto.CurrentCharges = m.CurrentCharges; dto.Effect = m.Effect;
                    break;
                case PotionItem p:
                    dto.Effect = p.Effect; dto.Duration = p.Duration;
                    dto.HealDiceCount = p.HealDiceCount; dto.HealDiceSides = p.HealDiceSides; dto.HealBonus = p.HealBonus;
                    break;
                case AmmunitionItem am:
                    dto.AmmunitionType = am.AmmunitionType; dto.DamageBonusOverride = am.DamageBonusOverride;
                    break;
                case ToolItem t:
                    dto.ToolType = t.ToolType; dto.HasProficiency = t.HasProficiency; dto.AssociatedSkill = t.AssociatedSkill;
                    break;
                case MountItem mo:
                    dto.SpeedFt = mo.SpeedFt; dto.CarryingCapacity = mo.CarryingCapacity; dto.MountType = mo.MountType;
                    break;
            }
            return dto;
        }
    }
}