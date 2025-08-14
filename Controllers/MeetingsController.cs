using IEEE.Data;
using IEEE.DTO.CommitteeDto;
using IEEE.DTO.MeetingDto;
using IEEE.DTO.UserDTO;
using IEEE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeetingsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllMeetingsDto>>> GetMeetings()
        {
            var meetings = await _context.Meetings
                .Include(m => m.Committee)
                .Include(m => m.Head)
                .Include(m => m.Users_Meetings)
                    .ThenInclude(um => um.User) // نجيب بيانات اليوزر
                .Select(m => new GetAllMeetingsDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    CommitteeName = m.Committee != null ? m.Committee.Name : null,
                    HeadUserName = m.Head != null ? m.Head.UserName : null,
                    Users = m.Users_Meetings.Select(um => new GetUsersDto
                    {
                        Id = um.User.Id,
                        RoleId = um.User.RoleId,
                        Eamil = um.User.Email,
                        IsActive = um.User.IsActive
                    }).ToList()
                })
                .ToListAsync();

            return Ok(meetings);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GetMeetingDto>> GetMeeting(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.Committee)
                .Include(m => m.Head)
                .Include(m => m.Users_Meetings)
                    .ThenInclude(um => um.User)
                .Where(m => m.Id == id)
                .Select(m => new GetMeetingDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Recap = m.Recap,
                    Committee = m.Committee != null
                        ? new CommitteeGetDto
                        {
                            Id = m.Committee.Id,
                            Name = m.Committee.Name
                        }
                        : null,
                    Head = m.Head != null
                        ? new GetUsersDto
                        {
                            Id = m.Head.Id,
                            Eamil = m.Head.Email
                        }
                        : null,
                    Users = m.Users_Meetings
                        .Select(um => new GetUsersDto
                        {
                            Id = um.User.Id,
                            Eamil = um.User.Email,
                            RoleId = um.User.RoleId,
                            IsActive = um.User.IsActive
                            // CommitteeId = um.User.CommitteeId
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (meeting == null)
                return NotFound();

            return Ok(meeting);
        }

        [HttpPost]
        public async Task<ActionResult<Meeting>> PostMeeting(CreateMeetingDto dto)
        {
            // تحقق من وجود اللجنة
            var committee = await _context.Committees.FindAsync(dto.CommitteeId);
            if (committee == null)
                return BadRequest("Committee not found.");

            // تحقق من وجود الرئيس
            var head = await _context.Users.FindAsync(dto.HeadId);
            if (head == null)
                return BadRequest("Head user not found.");

            // تحقق من وجود المستخدمين
            var users = await _context.Users
                .Where(u => dto.UserIds.Contains(u.Id))
                .ToListAsync();

            if (users.Count != dto.UserIds.Count)
                return BadRequest("Some users not found.");

            // أنشئ الاجتماع
            var meeting = new Meeting
            {
                Title = dto.Title,
                Description = dto.Description,
                Recap = dto.Recap,
                CommitteeId = dto.CommitteeId,
                HeadId = dto.HeadId,
                Users_Meetings = users.Select(u => new Users_Meetings
                {
                    UserId = u.Id
                }).ToList()
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMeeting), new { id = meeting.Id }, meeting);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeeting(int id, CreateMeetingDto dto)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
                return NotFound("Meeting not found.");

            // تحديث القيم
            meeting.Title = dto.Title;
            meeting.Description = dto.Description;
            meeting.CommitteeId = dto.CommitteeId;
            meeting.Recap = dto.Recap;
            meeting.HeadId= dto.HeadId;

            // حفظ التغييرات
            await _context.SaveChangesAsync();

            return NoContent();
        }

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

    }
}
