using IEEE.Data;
using IEEE.DTO.EventsDTO;
using IEEE.Entities;
using IEEE.Extenstions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [ApiController]
    [Route("api/Events")]
    //[Authorize(Roles = "High Board")]
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
        public async Task<ActionResult<PageRespone<EventResponse>>> GetAll([FromQuery] PaginationParams @params)
        {
            var query = GetBaseQuery();
            var totalRecords = await query.CountAsync();

            var events = await query
                .OrderByDescending(e => e.CreatedAt)
                .ApplyPagination(@params.PageNumber, @params.PageSize)
                .ToListAsync();

            return Ok(new PageRespone<EventResponse>(MapToResponseList(events), @params.PageNumber, @params.PageSize, totalRecords));
        }

        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult<PageRespone<EventResponse>>> GetUpcoming([FromQuery] PaginationParams @params)
        {
            var now = DateTime.UtcNow;
            var query = GetBaseQuery()
                .Where(e => e.IsCommingSoon || (e.StartDate.HasValue && e.StartDate.Value > now));

            var totalRecords = await query.CountAsync();
            var events = await query
                .OrderBy(e => e.StartDate)
                .ApplyPagination(@params.PageNumber, @params.PageSize)
                .ToListAsync();

            return Ok(new PageRespone<EventResponse>(MapToResponseList(events), @params.PageNumber, @params.PageSize, totalRecords));
        }

        [HttpGet("ongoing")]
        [AllowAnonymous]
        public async Task<ActionResult<PageRespone<EventResponse>>> GetOngoing([FromQuery] PaginationParams @params)
        {
            var now = DateTime.UtcNow;
            var query = GetBaseQuery()
                .Where(e => e.StartDate <= now && e.EndDate >= now);

            var totalRecords = await query.CountAsync();
            var events = await query
                .OrderBy(e => e.EndDate) 
                .ApplyPagination(@params.PageNumber, @params.PageSize)
                .ToListAsync();

            return Ok(new PageRespone<EventResponse>(MapToResponseList(events), @params.PageNumber, @params.PageSize, totalRecords));
        }

        [HttpGet("past")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventResponse>>> GetPast([FromQuery] PaginationParams @params)
        {
            var now = DateTime.UtcNow;
            var query = GetBaseQuery()
                .Where(e => e.EndDate.HasValue && e.EndDate.Value < now);

            var totalRecords = await query.CountAsync();
            var events = await query
                .OrderByDescending(e => e.EndDate)
                .ApplyPagination(@params.PageNumber, @params.PageSize)
                .ToListAsync();

            return Ok(new PageRespone<EventResponse>(MapToResponseList(events), @params.PageNumber, @params.PageSize, totalRecords));
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEventRequest request)
        {
            var evt = await _dbContext.Events.FindAsync(id);
            if (evt == null)
                return NotFound();

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _dbContext.EventCategories.AnyAsync(c => c.Id == request.CategoryId.Value);
                if (!categoryExists)
                    return BadRequest(new { error = "The specified CategoryId does not exist." });
            }

            var sanitizedRequest = request;

            if (sanitizedRequest.IsCommingSoon)
            {
                sanitizedRequest = sanitizedRequest with { StartDate = null, EndDate = null };
            }

            try
            {
                evt.Update(
                    sanitizedRequest.Name,
                    sanitizedRequest.KeyWords,
                    sanitizedRequest.StartDate,
                    sanitizedRequest.EndDate,
                    sanitizedRequest.IsCommingSoon,
                    sanitizedRequest.CategoryId ?? evt.CategoryId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

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


        #region Helper Methods
        private IQueryable<Event> GetBaseQuery()
        {
            return _dbContext.Events
                .Include(e => e.Category) 
                .AsNoTracking();
        }

        private IEnumerable<EventResponse> MapToResponseList(IEnumerable<Event> events)
        {
            return events.Select(e => MapToResponse(e, e.Category.Name));
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
        #endregion


    }
}