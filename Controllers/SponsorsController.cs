using IEEE.Data;
using IEEE.DTO.SponsorDto;
using IEEE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SponsorsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private string GetBaseUrl()
        {
            var req = HttpContext.Request;
            var pathBase = req.PathBase.HasValue ? req.PathBase.Value : string.Empty;
            return $"{req.Scheme}://{req.Host}{pathBase}";
        }

        private string? ToAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            return GetBaseUrl() + relativePath;
        }

        private GetSponsorDto MapToDto(Sponsor sponsor)
        {
            return new GetSponsorDto
            {
                Id          = sponsor.Id,
                Name        = sponsor.Name,
                Description = sponsor.Description,
                Date        = sponsor.Date,
                Image       = ToAbsoluteUrl(sponsor.Image)
            };
        }

        // ─── Save image helper ──────────────────────────────────────────────────

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Invalid image format. Allowed: jpg, jpeg, png, gif, webp.");

            var sponsorsFolder = Path.Combine(_environment.WebRootPath, "sponsors"); if (!Directory.Exists(sponsorsFolder))
                Directory.CreateDirectory(sponsorsFolder);

            var fileName = Guid.NewGuid() + ext;
            var filePath = Path.Combine(sponsorsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return "/sponsors/" + fileName; // relative path stored in DB
        }

        private void DeleteImage(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                        relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        // ─── GET api/Sponsors ─────────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetSponsorDto>>> GetAll()
        {
            var sponsors = await _context.Sponsors.ToListAsync();
            var result = sponsors.Select(MapToDto);
            return Ok(result);
        }

        // ─── GET api/Sponsors/{id} ────────────────────────────────────────────

        [HttpGet("{id}")]
        public async Task<ActionResult<GetSponsorDto>> GetById(int id)
        {
            var sponsor = await _context.Sponsors.FindAsync(id);
            if (sponsor == null) return NotFound(new { message = $"Sponsor with id {id} not found." });

            return Ok(MapToDto(sponsor));
        }

        // ─── POST api/Sponsors ────────────────────────────────────────────────

        //[Authorize(Roles = "High Board,Head,Vice,HR")]
        //[Authorize(Policy = "ActiveUserOnly")]
        [HttpPost]
        public async Task<ActionResult<GetSponsorDto>> Create([FromForm] CreateSponsorDto dto)
        {
            string? imagePath;
            try
            {
                imagePath = await SaveImageAsync(dto.Image);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var sponsor = new Sponsor
            {
                Name        = dto.Name,
                Description = dto.Description,
                Date        = dto.Date ?? DateTime.UtcNow,
                Image       = imagePath
            };

            _context.Sponsors.Add(sponsor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = sponsor.Id }, MapToDto(sponsor));
        }

        // ─── PUT api/Sponsors/{id} ────────────────────────────────────────────

        //[Authorize(Roles = "High Board,Head,Vice,HR")]
        //[Authorize(Policy = "ActiveUserOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateSponsorDto dto)
        {
            var sponsor = await _context.Sponsors.FindAsync(id);
            if (sponsor == null) return NotFound(new { message = $"Sponsor with id {id} not found." });

            // If a new image is supplied, replace the old one
            if (dto.Image != null && dto.Image.Length > 0)
            {
                string? newImagePath;
                try
                {
                    newImagePath = await SaveImageAsync(dto.Image);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }

                DeleteImage(sponsor.Image); // remove old file from disk
                sponsor.Image = newImagePath;
            }

            sponsor.Name        = dto.Name;
            sponsor.Description = dto.Description;
            sponsor.Date        = dto.Date ?? sponsor.Date;

            await _context.SaveChangesAsync();
            return Ok(MapToDto(sponsor));
        }

        // ─── DELETE api/Sponsors/{id} ─────────────────────────────────────────

        //[Authorize(Roles = "High Board,Head,Vice,HR")]
        //[Authorize(Policy = "ActiveUserOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sponsor = await _context.Sponsors.FindAsync(id);
            if (sponsor == null) return NotFound(new { message = $"Sponsor with id {id} not found." });

            DeleteImage(sponsor.Image); // remove image file from disk

            _context.Sponsors.Remove(sponsor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
