using IEEE.Data;
using IEEE.DTO.SubsectionDto;
using IEEE.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubsectionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubsectionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Subsections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetSubsection>>> GetSubsections()
        {
            var subsections = await _context.Subsections
                .Include(s => s.Article)
                .Select(s => new GetSubsection
                {
                    Id = s.Id,
                    Subtitle = s.Subtitle,
                    Paragraph = s.Paragraph,
                    Photo = s.Photo,
                    ArticleId = s.ArticleId,
                })
                .ToListAsync();

            return Ok(subsections);
        }

        // GET: api/Subsections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetSubsection>> GetSubsection(int id)
        {
            var subsection = await _context.Subsections
                .Include(s => s.Article)
                .Where(s => s.Id == id)
                .Select(s => new GetSubsection
                {
                    Id = s.Id,
                    Subtitle = s.Subtitle,
                    Paragraph = s.Paragraph,
                    Photo = s.Photo,
                    ArticleId = s.ArticleId,
                })
                .FirstOrDefaultAsync();

            if (subsection == null)
            {
                return NotFound();
            }

            return Ok(subsection);
        }


        // POST: api/Subsections
        [HttpPost]
        public async Task<IActionResult> CreateSubsection([FromForm] CreateSubsectionDto createSubsectionDto)
        {
            var articleExists = await _context.Articles.AnyAsync(a => a.Id == createSubsectionDto.ArticleId);
            if (!articleExists)
            {
                return BadRequest("Article does not exist");
            }

            string photoPath = null;
            if (createSubsectionDto.Photo != null && createSubsectionDto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/subsections");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createSubsectionDto.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createSubsectionDto.Photo.CopyToAsync(stream);
                }

                photoPath = "/uploads/subsections/" + fileName;
            }

            var subsection = new Subsection
            {
                Subtitle = createSubsectionDto.Subtitle,
                Paragraph = createSubsectionDto.Paragraph,
                Photo = photoPath,
                ArticleId = createSubsectionDto.ArticleId
            };

            _context.Subsections.Add(subsection);
            await _context.SaveChangesAsync();

            await _context.Entry(subsection).Reference(s => s.Article).LoadAsync();

            var subsectionDto = new GetSubsection
            {
                Id = subsection.Id,
                Subtitle = subsection.Subtitle,
                Paragraph = subsection.Paragraph,
                Photo = subsection.Photo,
                ArticleId = subsection.ArticleId
            };

            return CreatedAtAction(nameof(GetSubsection), new { id = subsection.Id }, subsectionDto);
        }


        // PUT: api/Subsections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubsection(int id, [FromForm] CreateSubsectionDto updateSubsectionDto)
        {
            var subsection = await _context.Subsections.FindAsync(id);
            if (subsection == null)
            {
                return NotFound();
            }

            var articleExists = await _context.Articles.AnyAsync(a => a.Id == updateSubsectionDto.ArticleId);
            if (!articleExists)
            {
                return BadRequest("Article does not exist");
            }

            subsection.Subtitle = updateSubsectionDto.Subtitle;
            subsection.Paragraph = updateSubsectionDto.Paragraph;
            subsection.ArticleId = updateSubsectionDto.ArticleId;

            if (updateSubsectionDto.Photo != null && updateSubsectionDto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/subsections");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateSubsectionDto.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateSubsectionDto.Photo.CopyToAsync(stream);
                }

                subsection.Photo = "/uploads/subsections/" + fileName;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/Subsections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubsection(int id)
        {
            var subsection = await _context.Subsections.FindAsync(id);
            if (subsection == null)
            {
                return NotFound();
            }

            _context.Subsections.Remove(subsection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubsectionExists(int id)
        {
            return _context.Subsections.Any(e => e.Id == id);
        }
    }
}
