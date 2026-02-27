using IEEE.Data;
using IEEE.DTO.EventsDTO;
using IEEE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [ApiController]
    [Route("api/Events")]
    [Authorize(Roles = "High Board,Head,Vice,HR")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public EventsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            // 1. Verify the category actually exists
            var categoryExists = await _dbContext.EventCategories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                return BadRequest(new { error = "The specified CategoryId does not exist." });

            var sanitizedRequest = request;

            if (sanitizedRequest.IsCommingSoon)
            {
                // When coming soon, dates must be null to satisfy the domain rule
                sanitizedRequest = sanitizedRequest with { StartDate = null, EndDate = null };
            }

            var newEvent = Event.Create(
                sanitizedRequest.Name,
                sanitizedRequest.KeyWords,
                sanitizedRequest.StartDate,
                sanitizedRequest.EndDate,
                sanitizedRequest.IsCommingSoon,
                sanitizedRequest.CategoryId);

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync();

            var category = await _dbContext.EventCategories.FindAsync(request.CategoryId);
            var response = MapToResponse(newEvent, category?.Name ?? "Unknown");

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetAll()
        {
            var events = await _dbContext.Events
                .Include(e => e.Category) 
                .AsNoTracking()
                .ToListAsync();

            var responseList = events.Select(e => MapToResponse(e, e.Category.Name));
            return Ok(responseList);
        }

        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetUpcoming()
        {
            var now = DateTime.UtcNow;
            var events = await _dbContext.Events
                .Include(e => e.Category)
                .AsNoTracking()
                .Where(e => e.IsCommingSoon || (e.StartDate.HasValue && e.StartDate.Value > now))
                .ToListAsync();

            var responseList = events.Select(e => MapToResponse(e, e.Category.Name));
            return Ok(responseList);
        }

        [HttpGet("past")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetPast()
        {
            var now = DateTime.UtcNow;
            var events = await _dbContext.Events
                .Include(e => e.Category)
                .AsNoTracking()
                .Where(e => e.EndDate.HasValue && e.EndDate.Value < now)
                .ToListAsync();

            var responseList = events.Select(e => MapToResponse(e, e.Category.Name));
            return Ok(responseList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponse>> GetById(Guid id)
        {
            var evt = await _dbContext.Events
                .Include(e => e.Category) 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evt == null)
                return NotFound();

            return Ok(MapToResponse(evt, evt.Category.Name));
        }

        [HttpPut("{id}/rename")]
        public async Task<IActionResult> Rename(Guid id, [FromBody] RenameEventRequest request)
        {
            var evt = await _dbContext.Events.FindAsync(id);
            if (evt == null) return NotFound();

            evt.Rename(request.NewName);

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/keywords")]
        public async Task<IActionResult> UpdateKeyWords(Guid id, [FromBody] UpdateEventKeyWordsRequest request)
        {
            var evt = await _dbContext.Events.FindAsync(id);
            if (evt == null) return NotFound();

            evt.UpdateKeyWords(request.KeyWords);

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/dates")]
        public async Task<IActionResult> UpdateDates(Guid id, [FromBody] UpdateEventDatesRequest request)
        {
            var evt = await _dbContext.Events.FindAsync(id);
            if (evt == null) return NotFound();

            var sanitizedRequest = request;

            if (sanitizedRequest.IsCommingSoon)
            {
                // Normalize to null dates when the event is marked as coming soon
                sanitizedRequest = sanitizedRequest with { StartDate = null, EndDate = null };
            }

            evt.UpdateDates(
                sanitizedRequest.StartDate,
                sanitizedRequest.EndDate,
                sanitizedRequest.IsCommingSoon);

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var evt = await _dbContext.Events.FindAsync(id);
            if (evt == null)
                return NotFound();

            _dbContext.Events.Remove(evt);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private static EventResponse MapToResponse(Event evt, string categoryName)
        {
            return new EventResponse(
                evt.Id,
                evt.Name,
                evt.KeyWords,
                evt.StartDate,
                evt.EndDate,
                evt.IsCommingSoon,
                evt.CategoryId,
                categoryName,
                evt.CreatedAt,
                evt.LastUpdatedAt);
        }
    }
}