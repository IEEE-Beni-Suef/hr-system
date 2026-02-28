using IEEE.Data;
using IEEE.DTO.EventCategoriesDTO;
using IEEE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [Route("api/EventCategories")]
    [ApiController]
    [Authorize(Roles = "High Board")]
    public class EventCategoriesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public EventCategoriesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventCategoryRequest request)
        {
            var category = EventCategory.Create(request.Name, request.Description);

            _dbContext.EventCategories.Add(category);
            await _dbContext.SaveChangesAsync();

            var response = MapToResponse(category);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventCategoryResponse>>> GetAll()
        {
            var categories = await _dbContext.EventCategories
                .AsNoTracking() 
                .ToListAsync();

            var responseList = categories.Select(MapToResponse);
            return Ok(responseList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventCategoryResponse>> GetById(Guid id)
        {
            var category = await _dbContext.EventCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            return Ok(MapToResponse(category));
        }

        [HttpPut("{id}/rename")]
        public async Task<IActionResult> Rename(Guid id, [FromBody] RenameEventCategoryRequest request)
        {
            var category = await _dbContext.EventCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            category.Rename(request.NewName);

            await _dbContext.SaveChangesAsync();
            return NoContent(); 
        }

        [HttpPut("{id}/description")]
        public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] UpdateCategoryDescriptionRequest request)
        {
            var category = await _dbContext.EventCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            category.UpdateDescription(request.NewDescription);

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _dbContext.EventCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            _dbContext.EventCategories.Remove(category);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { error = "Cannot delete this category because it contains events." });
            }

            return NoContent();
        }

        private static EventCategoryResponse MapToResponse(EventCategory category)
        {
            return new EventCategoryResponse(
                category.Id,
                category.Name,
                category.Description,
                category.CreatedAt,
                category.LastUpdatedAt);
        }
    }


}
