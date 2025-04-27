//using IEEE.Data;
//using IEEE.Entities;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace IEEE.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MeetingsController : ControllerBase
//    {
//        private readonly AppDbContext _context;


//        public MeetingsController(AppDbContext context)
//        {
//            _context = context;
//        }

        
//        ////[HttpGet]
//        ////public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
//        ////{
//        ////    return await _context.Meetings
//        ////        .Include(m => m.Creator)
//        ////        .Include(m => m.Committee)
//        ////        .ToListAsync();
//        ////}

        
//        //[HttpGet("{id}")]
//        //public async Task<ActionResult<Meeting>> GetMeeting(int id)
//        //{
//        //    var meeting = await _context.Meetings
//        //        .Include(m => m.Creator)
//        //        .Include(m => m.Committee)
//        //        .FirstOrDefaultAsync(m => m.Id == id);

//        //    if (meeting == null)
//        //        return NotFound();

//        //    return meeting;
//        //}

        
//        //[HttpPost]
//        //public async Task<ActionResult<Meeting>> PostMeeting(Meeting meeting)
//        //{
//        //    _context.Meetings.Add(meeting);
//        //    await _context.SaveChangesAsync();

//        //    return CreatedAtAction(nameof(GetMeeting), new { id = meeting.Id }, meeting);
//        //}

       
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutMeeting(int id, Meeting meeting)
//        {
//            if (id != meeting.Id)
//                return BadRequest();

//            _context.Entry(meeting).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.Meetings.Any(m => m.Id == id))
//                    return NotFound();
//                else
//                    throw;
//            }

//            return NoContent();
//        }

        
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteMeeting(int id)
//        {
//            var meeting = await _context.Meetings.FindAsync(id);
//            if (meeting == null)
//                return NotFound();

//            _context.Meetings.Remove(meeting);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
