using IEEE.Data;
using IEEE.DTO.ArticleDto;
using IEEE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        // Helper: Base URL
        private string GetBaseUrl()
        {
            var req = HttpContext.Request;
            var pathBase = req.PathBase.HasValue ? req.PathBase.Value : string.Empty;
            return $"{req.Scheme}://{req.Host}{pathBase}";
        }

        // Helper: relative "/uploads/.." → absolute "https://host/uploads/.."
        private string? ToAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            return GetBaseUrl() + relativePath;
        }

        // Helper: "a,b, c" → ["a","b","c"]
        private static string[] ToKeywordsArray(string? csv)
        {
            return string.IsNullOrWhiteSpace(csv)
                ? Array.Empty<string>()
                : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(k => k.Trim())
                     .Where(k => k.Length > 0)
                     .ToArray();
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetArticle>>> GetArticles()
        {
            var list = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Subsections)
                .ToListAsync(); // ← هنحوّل في الذاكرة عشان نقدر نعمل Split + BaseUrl

            var baseUrl = GetBaseUrl();

            var result = list.Select(a => new GetArticle
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Keywords = ToKeywordsArray(a.Keywords),
                Photo = string.IsNullOrEmpty(a.Photo) ? null : baseUrl + a.Photo,
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name
            });

            return Ok(result);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetArticle>> GetArticle(int id)
        {
            var a = await _context.Articles
                .Include(x => x.Category)
                .Include(x => x.Subsections)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (a == null) return NotFound();

            var dto = new GetArticle
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Keywords = ToKeywordsArray(a.Keywords),
                Photo = ToAbsoluteUrl(a.Photo),
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name
            };

            return Ok(dto);
        }

        // GET: api/Articles/5/show  (بيرجع الـ subsections كمان)
        [HttpGet("{id}/show")]
        public async Task<ActionResult<object>> ShowArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Subsections)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) return NotFound();

            var result = new
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Keywords = ToKeywordsArray(article.Keywords),
                Photo = ToAbsoluteUrl(article.Photo),
                Category = new
                {
                    CategoryId = article.Category?.Id,
                    Name = article.Category?.Name
                },
                Subsections = article.Subsections.Select(s => new
                {
                    Id = s.Id,
                    Subtitle = s.Subtitle,
                    Paragraph = s.Paragraph,
                    Photo = ToAbsoluteUrl(s.Photo)
                }).ToArray()
            };

            return Ok(result);
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<ActionResult<GetArticle>> CreateArticle([FromForm] CreateArticleDto createArticleDto)
        {
            // Check category
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == createArticleDto.CategoryId);
            if (!categoryExists)
                return BadRequest("Category does not exist");

            // Save photo if provided
            string? photoPath = null;
            if (createArticleDto.Photo != null && createArticleDto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(createArticleDto.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createArticleDto.Photo.CopyToAsync(stream);
                }

                photoPath = "/uploads/" + fileName; // هيتخزن Relative
            }

            var article = new Article
            {
                Title = createArticleDto.Title,
                Description = createArticleDto.Description,
                Keywords = createArticleDto.Keywords, // بتتخزن CSV في الـ DB
                Photo = photoPath,
                CategoryId = createArticleDto.CategoryId
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            await _context.Entry(article).Reference(a => a.Category).LoadAsync();

            var dto = new GetArticle
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Keywords = ToKeywordsArray(article.Keywords),
                Photo = ToAbsoluteUrl(article.Photo),
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.Name
            };

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, dto);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromForm] CreateArticleDto updateArticleDto)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == updateArticleDto.CategoryId);
            if (!categoryExists)
                return BadRequest("Category does not exist");

            article.Title = updateArticleDto.Title;
            article.Description = updateArticleDto.Description;
            article.Keywords = updateArticleDto.Keywords; // نخزن CSV
            article.CategoryId = updateArticleDto.CategoryId;

            if (updateArticleDto.Photo != null && updateArticleDto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(updateArticleDto.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateArticleDto.Photo.CopyToAsync(stream);
                }

                article.Photo = "/uploads/" + fileName; // relative
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
