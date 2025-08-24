using IEEE.Data;
using IEEE.DTO.ArticleDto;
using IEEE.DTO.CategoryDto;
using IEEE.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCategory>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Articles)
                .Select(c => new GetCategory
                {
                    Id   = c.Id , 
                    Name = c.Name,
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCategory>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Articles)
                .Where(c => c.Id == id)
                .Select(c => new GetCategory
                {
                    Id = c.Id , 
                    Name = c.Name,
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // GET: api/Categories/5/articles
        [HttpGet("{id}/articles")]
        public async Task<ActionResult<IEnumerable<GetArticle>>> GetCategoryArticles(int id)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == id);
            if (!categoryExists)
            {
                return NotFound("Category not found");
            }

            var articles = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Subsections)
                .Where(a => a.CategoryId == id)
                .Select(a => new GetArticle
                {
                    Id = a.Id , 
                    Title = a.Title,
                    Description = a.Description,
                    Keywords = a.Keywords,
                    Photo = a.Photo,
                    CategoryId = a.CategoryId,
                })
                .ToListAsync();

            return Ok(articles);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<CreateCategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CreateCategoryDto
            {
                Name = category.Name,
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = updateCategoryDto.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

    }
}
