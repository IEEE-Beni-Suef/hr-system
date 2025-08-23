using IEEE.Data;
using IEEE.DTO.ArticleDto;
using IEEE.Entities;
using Microsoft.AspNetCore.Http;
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

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetArticle>>> GetArticles()
        {
            var articles = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Subsections)
                .Select(a => new GetArticle
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Keywords = a.Keywords,
                    Photo = a.Photo,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                })
                .ToListAsync();

            return Ok(articles);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetArticle>> GetArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Subsections)
                .Where(a => a.Id == id)
                .Select(a => new GetArticle
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Keywords = a.Keywords,
                    Photo = a.Photo,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                })
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        // GET: api/Articles/5/show - Special endpoint that returns article with subsections array
        [HttpGet("{id}/show")]
        public async Task<ActionResult<object>> ShowArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Subsections)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound();
            }

            var result = new
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Keywords = article.Keywords,
                Photo = article.Photo,
                Category = new
                {
                    CategoryId = article.Category.Id,
                    Name = article.Category.Name,
                },
                Subsections = article.Subsections.Select(s => new
                {
                    Id = s.Id,
                    Subtitle = s.Subtitle,
                    Paragraph = s.Paragraph,
                    Photo = s.Photo
                }).ToArray()
            };

            return Ok(result);
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<ActionResult<CreateArticleDto>> CreateArticle(CreateArticleDto createArticleDto)
        {
            // Check if category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == createArticleDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist");
            }

            var article = new Article 
            {
                Title = createArticleDto.Title,
                Description = createArticleDto.Description,
                Keywords = createArticleDto.Keywords,
                Photo = createArticleDto.Photo,
                CategoryId = createArticleDto.CategoryId
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            // Load the category name for response
            await _context.Entry(article)
                .Reference(a => a.Category)
                .LoadAsync();

            var articleDto = new GetArticle
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Keywords = article.Keywords,
                Photo = article.Photo,
                CategoryId = article.CategoryId,
                CategoryName = article.Category.Name,
            };

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, articleDto);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, CreateArticleDto updateArticleDto)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Check if category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updateArticleDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist");
            }

            article.Title = updateArticleDto.Title;
            article.Description = updateArticleDto.Description;
            article.Keywords = updateArticleDto.Keywords;
            article.Photo = updateArticleDto.Photo;
            article.CategoryId = updateArticleDto.CategoryId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

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
