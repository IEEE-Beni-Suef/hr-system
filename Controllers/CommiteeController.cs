using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IEEE.Data;
using IEEE.Entities;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitteesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommitteesController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Committee>>> GetCommittees()
        {
            return await _context.Committees.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Committee>> GetCommittee(int id)
        {
            var committee = await _context.Committees.FindAsync(id);

            if (committee == null)
            {
                return NotFound();
            }

            return committee;
        }

        [HttpPost]
        public async Task<ActionResult<Committee>> PostCommittee(Committee committee)
        {
            _context.Committees.Add(committee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommittee), new { id = committee.Id }, committee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommittee(int id, Committee committee)
        {
            if (id != committee.Id)
            {
                return BadRequest();
            }

            _context.Entry(committee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommitteeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommittee(int id)
        {
            var committee = await _context.Committees.FindAsync(id);
            if (committee == null)
            {
                return NotFound();
            }

            _context.Committees.Remove(committee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommitteeExists(int id)
        {
            return _context.Committees.Any(e => e.Id == id);
        }
    }
}
