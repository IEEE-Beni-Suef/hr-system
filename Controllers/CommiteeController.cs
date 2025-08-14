using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IEEE.Data;
using IEEE.Entities;
using IEEE.DTO.CommitteeDto;
using IEEE.DTO.MeetingsDto;

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
        public async Task<ActionResult<IEnumerable<CommitteeGetDto>>> GetCommittees()
        {
            var committees = await _context.Committees
         .Include(c => c.Users)
         .ToListAsync();

            var committeesDto = committees.Select(c => new CommitteeGetDto
            {
                Id = c.Id,
                Name = c.Name,
                HeadId = c.HeadId ?? 0 ,
                MemberCount = _context.Users.Count(u => u.CommitteeId == c.Id), // حساب فعلي من الجدول
                VicesId = c.Vices.Select(v => v.Id).ToList() , 
                ImageUrl = c.ImageUrl,
            });
            return Ok(committeesDto);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<CommitteeGetDto>> GetCommittee(int id)
        {
            var committee = await _context.Committees
                   .Include(c => c.Users)
                   .FirstOrDefaultAsync(c => c.Id == id);
            if (committee == null)
            {
                return NotFound();
            }
            var committeeDto = new CommitteeGetDto
            {
                Id = committee.Id,
                Name = committee.Name,
                HeadId = committee.HeadId ?? 0 ,
                MemberCount = _context.Users.Count(u => u.CommitteeId == u.Id), // حساب فعلي من الجدول
                VicesId = committee.Vices.Select(v => v.Id).ToList(),
                ImageUrl = committee.ImageUrl,

            };

            return Ok(committeeDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCommittee(CommitteeCreateDto committeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var committee = new Committee
            {
                Name = committeeDto.Name,
                HeadId = committeeDto.HeadId ?? null,
                memberCount = committeeDto.MemberCount,
                Vices = await _context.Users
                           .Where(u => committeeDto.VicesId.Contains(u.Id))
                           .ToListAsync(),
                ImageUrl = committeeDto.ImageUrl

            };
            await _context.Committees.AddAsync(committee);
            await _context.SaveChangesAsync();
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommittee(int id,CommitteeUpdateDto committeeUpdateDto)
        {
            // تحميل الكوميتي مع الـ Vices
            var committee = await _context.Committees
                .Include(c => c.Vices)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null)
                return NotFound();

            // تحديث البيانات الأساسية
            committee.Name = committeeUpdateDto.Name;
            committee.HeadId = committeeUpdateDto.HeadId ?? committee.HeadId;
            committee.memberCount = committeeUpdateDto.MemberCount ?? committee.memberCount;
            committee.ImageUrl = committeeUpdateDto.ImageUrl ?? committee.ImageUrl;

            // تحديث الـ Vices
            if (committeeUpdateDto.VicesId != null && committeeUpdateDto.VicesId.Any())
            {
                // مسح الـ Vices القديمة
                committee.Vices.Clear();

                // إضافة الـ Vices الجديدة من الـ IDs
                var vices = await _context.Users
                    .Where(u => committeeUpdateDto.VicesId.Contains(u.Id))
                    .ToListAsync();

                foreach (var vice in vices)
                {
                    committee.Vices.Add(vice);
                }
            }

            await _context.SaveChangesAsync();

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


    }
}
