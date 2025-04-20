using IEEE.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public AdminController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPut("ActivateUser/{id}")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            user.IsActive = true;
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded) return Ok("User activated successfully");

            return BadRequest("Activation failed");
        }
    }
}
