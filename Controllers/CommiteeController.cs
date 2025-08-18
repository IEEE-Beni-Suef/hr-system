using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IEEE.Data;
using IEEE.Entities;
using IEEE.DTO.CommitteeDto;
using IEEE.DTO.MeetingsDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   //[Authorize(Roles = "High Board,HR")]

    public class CommitteesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;



        public CommitteesController(AppDbContext context, IWebHostEnvironment env , Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
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

            // تحقق من وجود head
            var head = await _context.Users.FindAsync(committeeDto.HeadId);
            if (head == null)
                return BadRequest("Head user not found.");

            // تحقق أن دوره هو Head
            if (head.RoleId != 2)
                return BadRequest($"User {head.UserName} does not have the required role to be a head.");
            // التحقق من الفيسز
            var vices = await _context.Users
                .Where(u => committeeDto.VicesId.Contains(u.Id))
                .ToListAsync();

            foreach (var vice in vices)
            {
                // التحقق أن RoleId = 5
                if (vice.RoleId != 5)
                    return BadRequest($"User {vice.UserName} does not have the required role to be a vice.");
            }


            // تحقق أن الهيد مش موجود في لجنة تانية
            bool headExistsInAnotherCommittee = await _context.Committees
                .AnyAsync(c => c.HeadId == committeeDto.HeadId);

            if (headExistsInAnotherCommittee)
                return BadRequest($"User {head.UserName} is already assigned as a head in another committee.");


            var committee = new Committee
            {
                Name = committeeDto.Name,
                HeadId = committeeDto.HeadId ,
                Vices = await _context.Users
                           .Where(u => committeeDto.VicesId.Contains(u.Id))
                           .ToListAsync(),
                ImageUrl = committeeDto.ImageUrl

            };
            await _context.Committees.AddAsync(committee);
            await _context.SaveChangesAsync();
            return Created("", new { message = "Committee created successfully" });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommittee(int id, CommitteeUpdateDto committeeUpdateDto)
        {
            // تحميل اللجنة
            var committee = await _context.Committees
                .Include(c => c.Vices)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null)
                return NotFound();

            // ✅ تحقق من الـ Head
            if (committeeUpdateDto.HeadId.HasValue)
            {
                var head = await _context.Users.FindAsync(committeeUpdateDto.HeadId.Value);
                if (head == null)
                    return BadRequest("Head user not found.");

                if (head.RoleId != 2)
                    return BadRequest($"User {head.UserName} does not have the required role to be a head.");

                bool headExistsInAnotherCommittee = await _context.Committees
                    .AnyAsync(c => c.HeadId == committeeUpdateDto.HeadId.Value && c.Id != id);

                if (headExistsInAnotherCommittee)
                    return BadRequest($"User {head.UserName} is already assigned as a head in another committee.");
            }
            // ✅ تحقق من الـ Vices
            if (committeeUpdateDto.VicesId != null && committeeUpdateDto.VicesId.Any())
            {
                var vices = await _context.Users
                    .Where(u => committeeUpdateDto.VicesId.Contains(u.Id))
                    .ToListAsync();

                foreach (var vice in vices)
                {
                    if (vice.RoleId != 5)
                        return BadRequest($"User {vice.UserName} does not have the required role to be a vice.");

                    // تحقق إن الـ Vice مش في لجنة تانية
                    bool isViceInAnotherCommittee = await _context.Committees
                        .AnyAsync(c => c.Id != id && c.Vices.Any(v => v.Id == vice.Id));

                    if (isViceInAnotherCommittee)
                        return BadRequest($"User {vice.UserName} is already assigned as a vice in another committee.");
                }

                // مسح الـ Vices القديمة
                committee.Vices.Clear();

                // إضافة الـ Vices الجديدة
                foreach (var vice in vices)
                {
                    committee.Vices.Add(vice);
                }
            }

            // تحديث البيانات الأساسية
            committee.Name = committeeUpdateDto.Name;
            committee.HeadId = committeeUpdateDto.HeadId ?? committee.HeadId;
            committee.ImageUrl = committeeUpdateDto.ImageUrl ?? committee.ImageUrl;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommittee(int id)
        {
            var committee = await _context.Committees
             .Include(c => c.Users) // لو عندك navigation property
             .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null)
                return NotFound();

            // فك الارتباط
            var users = _context.Users.Where(u => u.CommitteeId == id);
            foreach (var user in users)
            {
                user.CommitteeId = null;
            }

            _context.Committees.Remove(committee);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //[HttpPost("image")]
        //public async Task<IActionResult> UploadCommitteeImage(int committeeId, IFormFile file)
        //{
        //    var committee = await _context.Committees.FindAsync(committeeId);
        //    if (committee == null)
        //        return NotFound("Committee not found");

        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded");

        //    var fileName = Path.GetFileName(file.FileName); // اسم الملف الأصلي
        //    var path = Path.Combine(_env.WebRootPath, "images", fileName); // مكان التخزين على السيرفر

        //    // إنشاء مجلد الصور لو مش موجود
        //    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images"));

        //    // حفظ الملف فعلياً في المجلد
        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // حفظ المسار في قاعدة البيانات
        //    committee.ImageUrl = "/images/" + fileName;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Image uploaded successfully", imageUrl = committee.ImageUrl });
        //}


    }
}
