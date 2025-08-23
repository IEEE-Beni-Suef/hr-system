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
        public async Task<IActionResult> CreateSubsection(CreateSubsectionDto createSubsectionDto)
        {
            // Check if article exists
            var articleExists = await _context.Articles.AnyAsync(a => a.Id == createSubsectionDto.ArticleId);
            if (!articleExists)
            {
                return BadRequest("Article does not exist");
            }

            var subsection = new Subsection
            {
                Subtitle = createSubsectionDto.Subtitle,
                Paragraph = createSubsectionDto.Paragraph,
                Photo = createSubsectionDto.Photo,
                ArticleId = createSubsectionDto.ArticleId
            };

            _context.Subsections.Add(subsection);
            await _context.SaveChangesAsync();

            // Load the article title for response
            await _context.Entry(subsection)
                .Reference(s => s.Article)
                .LoadAsync();

            var subsectionDto = new GetSubsection
            {
                Id = subsection.Id,
                Subtitle = subsection.Subtitle,
                Paragraph = subsection.Paragraph,
                Photo = subsection.Photo,
                ArticleId = subsection.ArticleId,
            };

            return CreatedAtAction(nameof(GetSubsection), new { id = subsection.Id }, subsectionDto);
        }

        // PUT: api/Subsections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubsection(int id, CreateSubsectionDto updateSubsectionDto)
        {
            var subsection = await _context.Subsections.FindAsync(id);
            if (subsection == null)
            {
                return NotFound();
            }

            // Check if article exists
            var articleExists = await _context.Articles.AnyAsync(a => a.Id == updateSubsectionDto.ArticleId);
            if (!articleExists)
            {
                return BadRequest("Article does not exist");
            }

            subsection.Subtitle = updateSubsectionDto.Subtitle;
            subsection.Paragraph = updateSubsectionDto.Paragraph;
            subsection.Photo = updateSubsectionDto.Photo;
            subsection.ArticleId = updateSubsectionDto.ArticleId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubsectionExists(id))
                {
                    return NotFound();
                }
                throw;
            }

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
