// ──────────────────────────────────────────────────────────────────────────────
// DIFF PATCH  –  ResourcesController.cs
//
// 1. Add IHubContext<ResourceHub> to the constructor.
// 2. After SaveChangesAsync() in the ShareResource action, broadcast the
//    resource DTO to all clients in the campaign group.
// ──────────────────────────────────────────────────────────────────────────────

// ── Constructor change ────────────────────────────────────────────────────────
//
// BEFORE:
//   private readonly ApplicationDbContext _context;
//   private readonly IFileStorageService _fileStorage;
//
//   public ResourcesController(ApplicationDbContext context, IFileStorageService fileStorage)
//   {
//       _context = context;
//       _fileStorage = fileStorage;
//   }
//
// AFTER:

using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Hubs;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorApp1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly IHubContext<ResourceHub> _resourceHub;   // ← NEW

        public ResourcesController(
            ApplicationDbContext context,
            IFileStorageService fileStorage,
            IHubContext<ResourceHub> resourceHub)                  // ← NEW
        {
            _context = context;
            _fileStorage = fileStorage;
            _resourceHub = resourceHub;                            // ← NEW
        }

        // GET: api/Resources/campaign/5
        [HttpGet("campaign/{campaignId}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetCampaignResources(int campaignId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);

            if (!isMember) return Forbid();

            var resources = await _context.Resources
                .Include(r => r.Campaign)
                .Include(r => r.UploadedBy)
                .Include(r => r.Shares).ThenInclude(s => s.User)
                .Include(r => r.Shares).ThenInclude(s => s.Character)
                .Include(r => r.Shares).ThenInclude(s => s.SharedBy)
                .Where(r => r.CampaignId == campaignId && r.IsActive)
                .Where(r => r.UploadedById == userId ||
                            r.Shares.Any(s => s.ShareType == ShareType.EntireCampaign) ||
                            r.Shares.Any(s => s.UserId == userId))
                .ToListAsync();

            return Ok(resources.Select(MapToResourceDto).ToList());
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetResource(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources
                .Include(r => r.Campaign)
                .Include(r => r.UploadedBy)
                .Include(r => r.Shares).ThenInclude(s => s.User)
                .Include(r => r.Shares).ThenInclude(s => s.Character)
                .Include(r => r.Shares).ThenInclude(s => s.SharedBy)
                .FirstOrDefaultAsync(r => r.ResourceId == id);

            if (resource == null) return NotFound();

            var hasAccess = resource.UploadedById == userId ||
                            resource.Shares.Any(s => s.ShareType == ShareType.EntireCampaign) ||
                            resource.Shares.Any(s => s.UserId == userId);

            if (!hasAccess) return Forbid();

            return Ok(MapToResourceDto(resource));
        }

        // GET: api/Resources/file/5
        [HttpGet("file/{id}")]
        public async Task<IActionResult> DownloadResource(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources
                .Include(r => r.Shares)
                .FirstOrDefaultAsync(r => r.ResourceId == id);

            if (resource == null) return NotFound();

            var hasAccess = resource.UploadedById == userId ||
                            resource.Shares.Any(s => s.ShareType == ShareType.EntireCampaign) ||
                            resource.Shares.Any(s => s.UserId == userId);

            if (!hasAccess) return Forbid();

            try
            {
                var (fileData, contentType) = await _fileStorage.GetFileAsync(resource.FilePath);
                return File(fileData, contentType, resource.FileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("File not found on server");
            }
        }

        // POST: api/Resources/upload
        [HttpPost("upload")]
        [RequestSizeLimit(52428800)]

        public async Task<ActionResult<ResourceDto>> UploadResource(
            [FromForm] CreateResourceDto createDto,
            [FromForm] IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            try
            {
                var (filePath, originalFileName) = await _fileStorage.SaveFileAsync(file, createDto.CampaignId);
                var resourceType = DetermineResourceType(file.ContentType);

                var resource = new Resource
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Type = resourceType,
                    FileName = originalFileName,
                    FilePath = filePath,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    CampaignId = createDto.CampaignId,
                    UploadedById = userId,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Resources.Add(resource);
                await _context.SaveChangesAsync();

                await _context.Entry(resource).Reference(r => r.Campaign).LoadAsync();
                await _context.Entry(resource).Reference(r => r.UploadedBy).LoadAsync();

                return CreatedAtAction(nameof(GetResource),
                    new { id = resource.ResourceId },
                    MapToResourceDto(resource));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        // POST: api/Resources/5/share
        [HttpPost("{id}/share")]
        public async Task<ActionResult> ShareResource(int id, ShareResourceDto shareDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources
                .Include(r => r.Shares)
                .FirstOrDefaultAsync(r => r.ResourceId == id);

            if (resource == null) return NotFound();

            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == resource.CampaignId &&
                                cm.UserId == userId &&
                                cm.Role == CampaignRole.DM);

            if (resource.UploadedById != userId && !isDM)
                return Forbid();

            // Remove existing shares
            _context.ResourceShares.RemoveRange(resource.Shares);

            if (shareDto.ShareType == "EntireCampaign")
            {
                _context.ResourceShares.Add(new ResourceShare
                {
                    ResourceId = id,
                    ShareType = ShareType.EntireCampaign,
                    SharedById = userId,
                    SharedDate = DateTime.UtcNow
                });
            }
            else if (shareDto.ShareType == "SpecificUsers" && shareDto.UserIds != null)
            {
                foreach (var targetUserId in shareDto.UserIds)
                {
                    var isInCampaign = await _context.CampaignMembers
                        .AnyAsync(cm => cm.CampaignId == resource.CampaignId && cm.UserId == targetUserId);

                    if (isInCampaign)
                    {
                        _context.ResourceShares.Add(new ResourceShare
                        {
                            ResourceId = id,
                            ShareType = ShareType.SpecificUsers,
                            UserId = targetUserId,
                            SharedById = userId,
                            SharedDate = DateTime.UtcNow
                        });
                    }
                }
            }
            else if (shareDto.ShareType == "SpecificCharacters" && shareDto.CharacterIds != null)
            {
                foreach (var characterId in shareDto.CharacterIds)
                {
                    var character = await _context.Characters
                        .FirstOrDefaultAsync(c => c.CharacterId == characterId &&
                                                  c.CampaignId == resource.CampaignId);

                    if (character != null)
                    {
                        _context.ResourceShares.Add(new ResourceShare
                        {
                            ResourceId = id,
                            ShareType = ShareType.SpecificCharacters,
                            CharacterId = characterId,
                            SharedById = userId,
                            SharedDate = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            // ── Reload full resource with navigation props for the DTO ─────────
            var updatedResource = await _context.Resources
                .Include(r => r.Campaign)
                .Include(r => r.UploadedBy)
                .Include(r => r.Shares).ThenInclude(s => s.User)
                .Include(r => r.Shares).ThenInclude(s => s.Character)
                .Include(r => r.Shares).ThenInclude(s => s.SharedBy)
                .FirstAsync(r => r.ResourceId == id);

            var dto = MapToResourceDto(updatedResource);

            // ── Broadcast to the campaign group via ResourceHub ───────────────
            await _resourceHub.Clients
                .Group($"campaign_{resource.CampaignId}")
                .SendAsync("ResourceShared", dto);

            return Ok(new { message = "Resource shared successfully" });
        }

        // PUT: api/Resources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(int id, UpdateResourceDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == resource.CampaignId &&
                                cm.UserId == userId &&
                                cm.Role == CampaignRole.DM);

            if (resource.UploadedById != userId && !isDM)
                return Forbid();

            resource.Name = updateDto.Name;
            resource.Description = updateDto.Description;
            resource.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Resources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return NotFound();

            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == resource.CampaignId &&
                                cm.UserId == userId &&
                                cm.Role == CampaignRole.DM);

            if (resource.UploadedById != userId && !isDM)
                return Forbid();

            await _fileStorage.DeleteFileAsync(resource.FilePath);
            resource.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static ResourceType DetermineResourceType(string contentType) => contentType switch
        {
            var t when t.StartsWith("image/") => ResourceType.Image,
            "application/pdf" => ResourceType.PDF,
            var t when t.Contains("word") => ResourceType.Document,
            var t when t.Contains("sheet") || t.Contains("excel") => ResourceType.Spreadsheet,
            _ => ResourceType.Other
        };

        private static ResourceDto MapToResourceDto(Resource resource) => new()
        {
            ResourceId = resource.ResourceId,
            Name = resource.Name,
            Description = resource.Description,
            Type = resource.Type.ToString(),
            FileName = resource.FileName,
            ContentType = resource.ContentType,
            FileSize = resource.FileSize,
            CampaignId = resource.CampaignId,
            CampaignName = resource.Campaign?.Name ?? "Unknown",
            UploadedById = resource.UploadedById,
            UploadedByName = resource.UploadedBy?.UserName ?? "Unknown",
            CreatedDate = resource.CreatedDate,
            LastModifiedDate = resource.LastModifiedDate,
            Shares = resource.Shares.Select(s => new ResourceShareDto
            {
                ResourceShareId = s.ResourceShareId,
                ResourceId = s.ResourceId,
                ShareType = s.ShareType.ToString(),
                UserId = s.UserId,
                Username = s.User?.UserName,
                CharacterId = s.CharacterId,
                CharacterName = s.Character?.Name,
                SharedDate = s.SharedDate,
                SharedById = s.SharedById,
                SharedByName = s.SharedBy?.UserName ?? "Unknown"
            }).ToList()
        };
    }
}