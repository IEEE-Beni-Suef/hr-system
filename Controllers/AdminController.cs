using IEEE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "High Board,HR")]

    public class AdminController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;

        public AdminController(Microsoft.AspNetCore.Identity.UserManager<User> userManager)
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


        // PUT: api/Users/SetUserActivation/{id}?isActive=true
        [HttpPut("SetUserActivation/{id}")]
        public async Task<IActionResult> SetUserActivation(string id, [FromQuery] bool isActive)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.IsActive = isActive;
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok($"User is now {(isActive ? "Active" : "Inactive")}");

            return BadRequest(result.Errors);
        }

    }
}
