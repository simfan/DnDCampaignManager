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
                .Where(i => i.CharacterId == characterId)
                .OrderBy(i => i.Category).ThenBy(i => i.Name)
                .Select(i => MapToDto(i))
                .ToListAsync();

            return Ok(items);
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
                .Where(i => i.CampaignId == campaignId)
                .OrderBy(i => i.Category).ThenBy(i => i.Name)
                .Select(i => MapToDto(i))
                .ToListAsync();

            return Ok(items);
        }

        // ── POST: api/Inventory ─────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult<InventoryItemDto>> CreateItem(CreateInventoryItemDto dto)
        {
            var userId = GetUserId();

            if (dto.CharacterId == null && dto.CampaignId == null)
                return BadRequest("Must specify either CharacterId or CampaignId.");

            if (dto.CharacterId != null && dto.CampaignId != null)
                return BadRequest("Cannot specify both CharacterId and CampaignId.");

            // Access check
            if (dto.CharacterId.HasValue)
            {
                var character = await _context.Characters.FindAsync(dto.CharacterId.Value);
                if (character == null) return NotFound("Character not found.");
                var isMember = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == character.CampaignId && cm.UserId == userId);
                if (!isMember) return Forbid();
            }
            else
            {
                var isMember = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == dto.CampaignId!.Value && cm.UserId == userId);
                if (!isMember) return Forbid();
            }

            var item = new InventoryItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Quantity = dto.Quantity,
                Weight = dto.Weight,
                Value = dto.Value,
                IsEquipped = dto.IsEquipped,
                CharacterId = dto.CharacterId,
                CampaignId = dto.CampaignId,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            await _context.Entry(item).Reference(i => i.Character).LoadAsync();
            await _context.Entry(item).Reference(i => i.Campaign).LoadAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.InventoryItemId }, MapToDto(item));
        }

        // ── GET: api/Inventory/item/5 ───────────────────────────────────────
        [HttpGet("item/{id}")]
        public async Task<ActionResult<InventoryItemDto>> GetItem(int id)
        {
            var userId = GetUserId();
            var item = await _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);

            if (item == null) return NotFound();
            if (!await CanAccessItem(item, userId)) return Forbid();

            return Ok(MapToDto(item));
        }

        // ── PUT: api/Inventory/item/5 ───────────────────────────────────────
        [HttpPut("item/{id}")]
        public async Task<ActionResult<InventoryItemDto>> UpdateItem(int id, UpdateInventoryItemDto dto)
        {
            var userId = GetUserId();
            var item = await _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);

            if (item == null) return NotFound();
            if (!await CanAccessItem(item, userId)) return Forbid();

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Category = dto.Category;
            item.Quantity = dto.Quantity;
            item.Weight = dto.Weight;
            item.Value = dto.Value;
            item.IsEquipped = dto.IsEquipped;
            item.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(MapToDto(item));
        }

        // ── DELETE: api/Inventory/item/5 ────────────────────────────────────
        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = GetUserId();
            var item = await _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);

            if (item == null) return NotFound();
            if (!await CanAccessItem(item, userId)) return Forbid();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ── POST: api/Inventory/transfer ────────────────────────────────────
        [HttpPost("transfer")]
        public async Task<ActionResult<InventoryItemDto>> Transfer(TransferInventoryItemDto dto)
        {
            var userId = GetUserId();

            if (dto.ToCharacterId == null && dto.ToCampaignId == null)
                return BadRequest("Must specify a destination.");

            if (dto.ToCharacterId != null && dto.ToCampaignId != null)
                return BadRequest("Cannot specify both destinations.");

            var item = await _context.InventoryItems
                .Include(i => i.Character)
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.InventoryItemId == dto.InventoryItemId);

            if (item == null) return NotFound();
            if (!await CanAccessItem(item, userId)) return Forbid();

            // Verify destination access
            if (dto.ToCharacterId.HasValue)
            {
                var destCharacter = await _context.Characters.FindAsync(dto.ToCharacterId.Value);
                if (destCharacter == null) return NotFound("Destination character not found.");
                var isMember = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == destCharacter.CampaignId && cm.UserId == userId);
                if (!isMember) return Forbid();
            }
            else
            {
                var isMember = await _context.CampaignMembers
                    .AnyAsync(cm => cm.CampaignId == dto.ToCampaignId!.Value && cm.UserId == userId);
                if (!isMember) return Forbid();
            }

            int qty = dto.QuantityToTransfer ?? item.Quantity;
            if (qty <= 0 || qty > item.Quantity)
                return BadRequest("Invalid quantity.");

            if (qty < item.Quantity)
            {
                // Partial transfer: reduce source, create new item at destination
                item.Quantity -= qty;
                item.LastUpdatedDate = DateTime.UtcNow;

                var newItem = new InventoryItem
                {
                    Name = item.Name,
                    Description = item.Description,
                    Category = item.Category,
                    Quantity = qty,
                    Weight = item.Weight,
                    Value = item.Value,
                    IsEquipped = false,
                    CharacterId = dto.ToCharacterId,
                    CampaignId = dto.ToCampaignId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow
                };
                _context.InventoryItems.Add(newItem);
                await _context.SaveChangesAsync();
                await _context.Entry(newItem).Reference(i => i.Character).LoadAsync();
                await _context.Entry(newItem).Reference(i => i.Campaign).LoadAsync();
                return Ok(MapToDto(newItem));
            }
            else
            {
                // Full transfer: move the item
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

        // ── Helpers ─────────────────────────────────────────────────────────

        private async Task<bool> CanAccessItem(InventoryItem item, string userId)
        {
            int campaignId = item.Character?.CampaignId ?? item.CampaignId ?? 0;
            return await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);
        }

        private static InventoryItemDto MapToDto(InventoryItem i) => new()
        {
            InventoryItemId = i.InventoryItemId,
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
    }
}