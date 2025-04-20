using IEEE.DTO;
using IEEE.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IEEE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HighBoard")] 
    public class UsersController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public UsersController(Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Users/GetAllUsers
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Name,
                    u.Email,
                    u.Faculty,
                    u.Role ,
                    u.IsActive
                })
                .ToList();

            return Ok(users);
        }

        // POST: api/Users/CreateUser
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Name = dto.Name,
                Email = dto.Email,
                Faculty = dto.Faculty,
                IsActive = false // أو false حسب الحالة
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
                return Ok("User created successfully");

            return BadRequest(result.Errors);
        }

        // PUT: api/Users/EditUser/{id}
        [HttpPut("EditUser/{id}")]
        public async Task<IActionResult> EditUser(string id, [FromBody] EditUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.Name = dto.Name;
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.Faculty = dto.Faculty;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("User updated successfully");

            return BadRequest(result.Errors);
        }

        // DELETE: api/Users/DeleteUser/{id}
        [HttpDelete("DeleteUser/{id}")]
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

        // PUT: api/Users/SetUserActivation/{id}?isActive=true
        [HttpPut("SetUserActivation/{id}")]
        public async Task<IActionResult> SetUserActivation(string id, [FromQuery] bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok($"User is now {(isActive ? "Active" : "Inactive")}");

            return BadRequest(result.Errors);
        }

    }
}

