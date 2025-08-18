using IEEE.Data;
using IEEE.DTO.CommitteeDto;
using IEEE.DTO.MeetingAttendents;
using IEEE.DTO.MeetingDto;
using IEEE.DTO.UserDTO;
using IEEE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "High Board,Head,Vice")]

    public class MeetingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeetingsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetMeetingDto>>> GetMeetings()
        {
            var meetings = await _context.Meetings
                .Include(m => m.Committee)
                .Include(m => m.Head)
                .Select(m => new GetMeetingDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Recap = m.Recap,
                    DateTime = m.DateTime,
                    Type = m.Type,
                    CommitteeName = m.Committee != null ? m.Committee.Name : null,
                    HeadName = m.Head != null ? m.Head.UserName : null,
                }).ToListAsync();

            return Ok(meetings);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GetMeetingDto>> GetMeeting(int id)
        {
            var meeting = _context.Meetings.Where(m => m.Id == id).
                Include(m=>m.Head)
                .Include(m=>m.Committee)
                .FirstOrDefault();
            if (meeting == null)
                return NotFound("Meeting not found.");

            var meetingDto = new GetMeetingDto
            {
                Id = meeting.Id,
                Title = meeting.Title,
                Description = meeting.Description,
                Recap = meeting.Recap,
                DateTime = meeting.DateTime,
                Type = meeting.Type,
                CommitteeName = meeting.Committee != null ? meeting.Committee.Name : null,
                HeadName = meeting.Head != null ? meeting.Head.UserName : null,

            };
            return Ok(meetingDto);

        }

        [HttpPost]
        public async Task<ActionResult> PostMeeting(CreateMeetingDto dto)
        {
            // تحقق من وجود اللجنة
            var committee = await _context.Committees.FindAsync(dto.CommitteeId);
            if (committee == null)
                return BadRequest("Committee not found.");

            // تحقق من وجود الرئيس
            var head = await _context.Users.FindAsync(dto.HeadId);
            if (head == null)
                return BadRequest("Head user not found.");

            // أنشئ الاجتماع
            var meeting = new Meeting
            {
                Title = dto.Title,
                Description = dto.Description,
                Recap = dto.Recap,
                DateTime = dto.DateTime,
                Type = dto.Type,
                CommitteeId = dto.CommitteeId,
                HeadId = dto.HeadId,
                  };

            await _context.Meetings.AddAsync(meeting);
            await _context.SaveChangesAsync();

            return Created();
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMeeting(int id, CreateMeetingDto dto)
        //{
        //    var meeting = await _context.Meetings.FindAsync(id);
        //    if (meeting == null)
        //        return NotFound("Meeting not found.");

        //    // تحديث القيم
        //    meeting.Title = dto.Title;
        //    meeting.Description = dto.Description;
        //    meeting.CommitteeId = dto.CommitteeId;
        //    meeting.Recap = dto.Recap;
        //    meeting.HeadId= dto.HeadId;

        //    // حفظ التغييرات
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            // هات الميتنج مع جدول Users_Meetings
            var meeting = await _context.Meetings
                .Include(m => m.Users_Meetings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
                return NotFound();

            // لو فيه علاقات، امسحها
            if (meeting.Users_Meetings.Any())
            {
                _context.Users_Meetings.RemoveRange(meeting.Users_Meetings);
                await _context.SaveChangesAsync();
            }

            // احذف الميتنج نفسه
            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("attendent")]
        public async Task<IActionResult> AddAttendent(CreateAttendentsDto createAttendentsDto)
        {
            // check meeting existance
            var meeting = await _context.Meetings.FindAsync(createAttendentsDto.MeetingId);
            if (meeting == null)
                return NotFound("Meeting not found.");

            foreach(var userAttendent in createAttendentsDto.UsersAttendents)
            {
                // check user existance
                var user = await _context.Users.FindAsync(userAttendent.UserId);
                if (user == null)
                    return BadRequest($"User with ID {userAttendent.UserId} not found.");
                // check if the user is already an attendent
                var existingAttendent = await _context.Users_Meetings
                    .FirstOrDefaultAsync(um => um.UserId == userAttendent.UserId && um.MeetingId == createAttendentsDto.MeetingId);
                if (existingAttendent != null)
                    continue; // skip if already exists
                // add the new attendent
                var users_Meeting = new Users_Meetings
                {
                    UserId = userAttendent.UserId,
                    MeetingId = createAttendentsDto.MeetingId,
                    IsAttend = userAttendent.isAttend,
                    Score = userAttendent.Score
                };
                await _context.Users_Meetings.AddAsync(users_Meeting);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("attendents/{meetingId}")]
        public async Task<ActionResult<IEnumerable<GetMeetingAttendentsDto>>> GetAttendents(int meetingId)
        {
            // check meeting existance
            var meeting = await _context.Meetings.FindAsync(meetingId);
            if (meeting == null)
                return NotFound($"Meeting with Id {meetingId} not found.");
            var attendents = await _context.Users_Meetings
                .Where(um => um.MeetingId == meetingId)
                .Include(um => um.User)
                .Select(um => new GetMeetingAttendentsDto
                {
                    UserId = um.UserId,
                    UserName = um.User.FName + " " + um.User.LName,
                    IsAttend = um.IsAttend,
                    Score = um.Score
                })
                .ToListAsync();
            return Ok(attendents);
        }
    }
}
