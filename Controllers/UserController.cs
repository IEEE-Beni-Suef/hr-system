using IEEE.Data;
using IEEE.DTO.UserDTO;
using IEEE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IEEE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize(Roles = "High Board,Head,Vice")]


    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(AppDbContext appDbContext,UserManager<User> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = appDbContext;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.Include(u => u.Committees).ToListAsync();

            var userdto =  new List <GetUsersDto>();

            foreach (var user in users)
            {
                var userRolesIds = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                var dto = new GetUsersDto
                {
                    Id = user.Id,
                    Eamil = user.Email,
                    IsActive = user.IsActive,
                    RoleId = user.RoleId  ,
                    CommitteesId = user.Committees.Select(c => c.Id).ToList() // كل اللجان
,
                };

                userdto.Add(dto);
            }

            return Ok(userdto);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userManager.Users
                .Include(u => u.Committees)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var userRolesIds = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var dto = new GetUsersDto
            {
                Id = user.Id,
                Eamil = user.Email,
                IsActive = user.IsActive,
                RoleId = user.RoleId,
                CommitteesId = user.Committees.Select(c => c.Id).ToList() // كل اللجان
            };

            return Ok(dto);
        }


        // POST: api/Users/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(createuserdto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                FName = dto.FirstName,
                MName = dto.MiddleName,
                LName = dto.LastName,
                Year = dto.Year,
                Sex = dto.Sex,
                Goverment = dto.Goverment,
                PhoneNumber = dto.Phone,
                Email = dto.Email,
                Faculty = dto.Faculty,
                IsActive = dto.IsActive,
                CommitteeId = dto.CommitteeIds != null && dto.CommitteeIds.Any() ? dto.CommitteeIds.FirstOrDefault() : null, // Assuming the first committee is assigned
                RoleId = dto.RoleId,
                PasswordHash = dto.Password

            };

            // التحقق من وجود الـ Role في AspNetRoles
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == dto.RoleId);

            if (!roleExists)
            {
                throw new ArgumentException($"Role with ID {dto.RoleId} does not exist in AspNetRoles.");
            }

            // التحقق من عدم تكرار الـ Username أو Email
            var userExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (userExists)
            {
                throw new ArgumentException("Email already exists.");
            }




            var result = await _userManager.CreateAsync(user, dto.Password);
            await _context.SaveChangesAsync();


            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }


            //_context.Users.Add(user);

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        // PUT: api/Users/EditUser/{id} 
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] EditUserDto dto)
        {

            // تحميل المستخدم مع الـ Committees
            var user = await _userManager.Users
                .Include(u => u.Committees)
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
                return NotFound("User not found");


            // التحقق من وجود الـ Role الجديد في AspNetRoles
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == dto.RoleId);

            if (!roleExists)
            {
                throw new ArgumentException($"Role with ID {dto.RoleId} does not exist in AspNetRoles.");
            }
            user.UserName = dto.Email;
            user.FName = dto.FirstName;
            user.MName = dto.MiddleName;
            user.LName = dto.LastName;
            user.Sex  = dto.Sex;
            user.PhoneNumber = dto.Phone;
            user.Goverment = dto.Goverment;
            user.Year = dto.Year; 
            user.Email = dto.Email;
            user.Faculty = dto.Faculty;
            user.RoleId = dto.RoleId;
            user.CommitteeId = dto.CommitteeIds.FirstOrDefault(); // Assuming the first committee is the primary one

            // 1. Load the selected committees from DB
            var selectedCommittees = await _context.Committees
                .Where(c => dto.CommitteeIds.Contains(c.Id))
                .ToListAsync();

            // 2. Clear current user committees
            user.Committees.Clear();

            // 3. Add the new selected committees
            foreach (var committee in selectedCommittees)
            {

                if (!user.Committees.Any(c => c.Id == committee.Id))
                {
                    user.Committees.Add(committee);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("User updated successfully");

            return BadRequest(result.Errors);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles =await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return Ok(roles);
        }

        // DELETE: api/Users/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");


            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok("User deleted successfully");

            return BadRequest(result.Errors);
        }

    }
}

