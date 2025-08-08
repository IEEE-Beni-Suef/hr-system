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
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
        {

            var meetings = await _context.Meetings
                .Include(m => m.Committee)
                .Include(m => m.Head)
                .Select(m => new GetAllMeetingsDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    CommitteeName = m.Committee.Name,
                    HeadUserName = m.Head.UserName ,
                    Users = m.Users.Select(u => new GetUsersDto
                    {
                        Id = u.Id,
                        RoleId = u.RoleId,  
                        Eamil = u.Email , 
                        IsActive = u.IsActive , 
                        CommitteeId = u.CommitteeId 

                    }).ToList()
                })
                .ToListAsync();

            return Ok(meetings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeeting(int id)
        {

            var meeting = await _context.Meetings
                .Include(m => m.Committee)
                .Include(m => m.Users)
                .Include(m => m.Head)
                .Where(m => m.Id == id)
                .Select(m => new GetMeetingDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Recap = m.Recap,
                    Committee = new CommitteeGetDto
                    {
                        Id = m.Committee.Id,
                        Name = m.Committee.Name
                    },
                    Head = new GetUsersDto
                    {
                        Id = m.Head.Id,
                        Eamil = m.Head.Email
                    },
                    Users = m.Users.Select(u => new GetUsersDto
                    {
                        Id = u.Id,
                        Eamil = u.Email ,
                        RoleId = u.RoleId,
                        IsActive = u.IsActive,
                        CommitteeId = u.CommitteeId
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
                Users = users
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostMeeting), new { id = meeting.Id }, meeting);
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
            // حذف العلاقات أولًا
            var meeting = await _context.Meetings
                .Include(m => m.Users)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meeting == null)
                return NotFound();

            // فك الارتباط مع المستخدمين (users)
            meeting.Users.Clear();

            // احفظ التغيير الأول عشان يفك العلاقة
            await _context.SaveChangesAsync();

            // بعدين احذف الميتنج
            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
