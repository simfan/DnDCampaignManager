using BlazorApp1.Data;
using BlazorApp1.DTOs;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ResourcesController(ApplicationDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // GET: api/Resources/campaign/5
        [HttpGet("campaign/{campaignId}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetCampaignResources(int campaignId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user is a member of this campaign
            var isMember = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            // Get resources that are either:
            // 1. Shared with entire campaign
            // 2. Shared specifically with this user
            // 3. Uploaded by this user
            var resources = await _context.Resources
                .Include(r => r.Campaign)
                .Include(r => r.UploadedBy)
                .Include(r => r.Shares)
                    .ThenInclude(s => s.User)
                .Include(r => r.Shares)
                    .ThenInclude(s => s.Character)
                .Include(r => r.Shares)
                    .ThenInclude(s => s.SharedBy)
                .Where(r => r.CampaignId == campaignId && r.IsActive)
                .Where(r => r.UploadedById == userId || // User uploaded it
                           r.Shares.Any(s => s.ShareType == ShareType.EntireCampaign) || // Shared with all
                           r.Shares.Any(s => s.UserId == userId)) // Shared with user specifically
                .ToListAsync();

            var resourceDtos = resources.Select(r => MapToResourceDto(r)).ToList();

            return Ok(resourceDtos);
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetResource(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources
                .Include(r => r.Campaign)
                .Include(r => r.UploadedBy)
                .Include(r => r.Shares)
                    .ThenInclude(s => s.User)
                .Include(r => r.Shares)
                    .ThenInclude(s => s.Character)
                .Include(r => r.Shares)
                    .ThenInclude(s => s.SharedBy)
                .FirstOrDefaultAsync(r => r.ResourceId == id);

            if (resource == null)
            {
                return NotFound();
            }

            // Check if user has access
            var hasAccess = resource.UploadedById == userId ||
                           resource.Shares.Any(s => s.ShareType == ShareType.EntireCampaign) ||
                           resource.Shares.Any(s => s.UserId == userId);

            if (!hasAccess)
            {
                return Forbid();
            }

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

            if (resource == null)
            {
                return NotFound();
            }

            // Check if user has access
            var hasAccess = resource.UploadedById == userId ||
                           resource.Shares.Any(s => s.ShareType == ShareType.EntireCampaign) ||
                           resource.Shares.Any(s => s.UserId == userId);

            if (!hasAccess)
            {
                return Forbid();
            }

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
        
        [RequestSizeLimit(52428800)] // 50MB limit
        public async Task<ActionResult<ResourceDto>> UploadResource([FromForm] CreateResourceDto createDto, [FromForm] IFormFile file)
        {
            //var userId = "22de933d-9e1e-4b46-a21b-625e5f9f836b";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verify user is a DM of this campaign
            /*var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == createDto.CampaignId &&
                               cm.UserId == userId &&
                               cm.Role == CampaignRole.DM);

            if (!isDM)
            {
                return Forbid("Only DMs can upload resources");
            }*/

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            try
            {
                // Save file
                var (filePath, originalFileName) = await _fileStorage.SaveFileAsync(file, createDto.CampaignId);

                // Determine resource type based on file extension
                var resourceType = DetermineResourceType(file.ContentType);

                // Create resource record
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

                // Load navigation properties
                await _context.Entry(resource)
                    .Reference(r => r.Campaign)
                    .LoadAsync();
                await _context.Entry(resource)
                    .Reference(r => r.UploadedBy)
                    .LoadAsync();

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

            if (resource == null)
            {
                return NotFound();
            }

            // Only DM or uploader can share
            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == resource.CampaignId &&
                               cm.UserId == userId &&
                               cm.Role == CampaignRole.DM);

            if (resource.UploadedById != userId && !isDM)
            {
                return Forbid();
            }

            // Remove existing shares
            _context.ResourceShares.RemoveRange(resource.Shares);

            // Add new shares based on type
            if (shareDto.ShareType == "EntireCampaign")
            {
                var share = new ResourceShare
                {
                    ResourceId = id,
                    ShareType = ShareType.EntireCampaign,
                    SharedById = userId,
                    SharedDate = DateTime.UtcNow
                };
                _context.ResourceShares.Add(share);
            }
            else if (shareDto.ShareType == "SpecificUsers" && shareDto.UserIds != null)
            {
                foreach (var targetUserId in shareDto.UserIds)
                {
                    // Verify user is in campaign
                    var isInCampaign = await _context.CampaignMembers
                        .AnyAsync(cm => cm.CampaignId == resource.CampaignId && cm.UserId == targetUserId);

                    if (isInCampaign)
                    {
                        var share = new ResourceShare
                        {
                            ResourceId = id,
                            ShareType = ShareType.SpecificUsers,
                            UserId = targetUserId,
                            SharedById = userId,
                            SharedDate = DateTime.UtcNow
                        };
                        _context.ResourceShares.Add(share);
                    }
                }
            }
            else if (shareDto.ShareType == "SpecificCharacters" && shareDto.CharacterIds != null)
            {
                foreach (var characterId in shareDto.CharacterIds)
                {
                    // Verify character is in campaign
                    var character = await _context.Characters
                        .FirstOrDefaultAsync(c => c.CharacterId == characterId &&
                                                 c.CampaignId == resource.CampaignId);

                    if (character != null)
                    {
                        var share = new ResourceShare
                        {
                            ResourceId = id,
                            ShareType = ShareType.SpecificCharacters,
                            CharacterId = characterId,
                            SharedById = userId,
                            SharedDate = DateTime.UtcNow
                        };
                        _context.ResourceShares.Add(share);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Resource shared successfully" });
        }

        // PUT: api/Resources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(int id, UpdateResourceDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resource = await _context.Resources.FindAsync(id);

            if (resource == null)
            {
                return NotFound();
            }

            // Only uploader or DM can update
            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == resource.CampaignId &&
                               cm.UserId == userId &&
                               cm.Role == CampaignRole.DM);

            if (resource.UploadedById != userId && !isDM)
            {
                return Forbid();
            }

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

            if (resource == null)
            {
                return NotFound();
            }

            // Only uploader or DM can delete
            var isDM = await _context.CampaignMembers
                .AnyAsync(cm => cm.CampaignId == resource.CampaignId &&
                               cm.UserId == userId &&
                               cm.Role == CampaignRole.DM);

            if (resource.UploadedById != userId && !isDM)
            {
                return Forbid();
            }

            // Delete file from storage
            await _fileStorage.DeleteFileAsync(resource.FilePath);

            // Soft delete or hard delete
            resource.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private ResourceDto MapToResourceDto(Resource resource)
        {
            return new ResourceDto
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

        private ResourceType DetermineResourceType(string contentType)
        {
            if (contentType.StartsWith("image/"))
                return ResourceType.Image;
            if (contentType.StartsWith("audio/"))
                return ResourceType.Audio;
            if (contentType == "application/pdf" ||
                contentType.Contains("document") ||
                contentType.Contains("text"))
                return ResourceType.Document;

            return ResourceType.Other;
        }
    }
}
