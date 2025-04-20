using IEEE.DTO;
using IEEE.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public AccountController(UserManager<User> UserManager)
        {
            userManager = UserManager;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto UserFromRequest)
        {
            if (ModelState.IsValid)
            {

                //save in DB 

                User user = new User();
                user.UserName = UserFromRequest.UserName;
                user.Name = UserFromRequest.Name;
                user.Faculty = UserFromRequest.Faculty;
                user.Email = UserFromRequest.Email;
                user.Role = UserFromRequest.Role;
                user.IsActive = false;




                
                IdentityResult result = await userManager.CreateAsync(user, UserFromRequest.Password);

                if (result.Succeeded)
                {
                    return Ok("created");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("Password", item.Description);
                }


            }

            return BadRequest(ModelState);


        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto userFromRequest)
        {
            if (ModelState.IsValid)
            {
                User userfromdb = await userManager.FindByNameAsync(userFromRequest.UserName);

                if (userfromdb != null)
                {
                    bool found = await userManager.CheckPasswordAsync(userfromdb, userFromRequest.Password);
                    if (found)
                    {
                        //  Check if user is active
                        if (!userfromdb.IsActive)
                        {
                            return Unauthorized(new { message = "Your account is not activated yet." });
                        }

                        List<Claim> UserClaim = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userfromdb.Id.ToString()),
                    new Claim(ClaimTypes.Name, userfromdb.UserName)
                };

                        var UserRoles = await userManager.GetRolesAsync(userfromdb);
                        foreach (var roleName in UserRoles)
                        {
                            UserClaim.Add(new Claim(ClaimTypes.Role, roleName));
                        }

                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            issuer: "http://localhost:5078/",
                            audience: "ieeeweb",
                            expires: DateTime.Now.AddHours(1),
                            claims: UserClaim,
                            signingCredentials: new SigningCredentials(
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KiraSuperUltraMegaSecretKey!1234567890")),
                                SecurityAlgorithms.HmacSha256
                            )
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(mytoken);

                        return Ok(new
                        {
                            token = tokenString
                        });
                    }
                }

                ModelState.AddModelError("Username", "Username OR Password Invalid");
                return Unauthorized(ModelState);
            }

            return BadRequest(ModelState);
        }


    }
}
