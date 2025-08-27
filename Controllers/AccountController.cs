using IEEE.Data;
using IEEE.DTO.UserDTO;
using IEEE.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> roleManager;
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private readonly IConfiguration config;

        public AccountController(Microsoft.AspNetCore.Identity.UserManager<User> UserManager, IConfiguration config, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> _roleManager, AppDbContext context)
        {
            userManager = UserManager;
            this.config = config;
            roleManager = _roleManager;
            _context = context;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto UserFromRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 1. التحقق من وجود الـ Role
                var role = await _context.Roles
                    .Where(r => r.Id == UserFromRequest.RoleId)
                    .FirstOrDefaultAsync();

                if (role == null)
                    return BadRequest($"Role with ID {UserFromRequest.RoleId} does not exist.");

                // 2. التحقق من عدم تكرار الـ Email
                var existingUser = await userManager.FindByEmailAsync(UserFromRequest.Email);
                if (existingUser != null)
                    return BadRequest("Email already exists.");

                // 3. إنشاء الـ User
                User user = new User
                {
                    UserName = UserFromRequest.Email,
                    Email = UserFromRequest.Email,
                    FName = UserFromRequest.FirstName,
                    MName = UserFromRequest.MiddleName,
                    LName = UserFromRequest.LastName,
                    Faculty = UserFromRequest.Faculty,
                    PhoneNumber = UserFromRequest.Phone,
                    Sex = UserFromRequest.Sex,
                    Goverment = UserFromRequest.Goverment,
                    Year = UserFromRequest.Year,
                    IsActive = false,
                    EmailConfirmed = false,
                    RoleId = UserFromRequest.RoleId // حط الـ RoleId
                };

                // 4. حفظ المستخدم الأول بدون إضافة للـ Role
                var result = await userManager.CreateAsync(user, UserFromRequest.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                // 5. إضافة الـ Role يدوياً في قاعدة البيانات
                var userRole = new Microsoft.AspNetCore.Identity.IdentityUserRole<int>
                {
                    UserId = user.Id,
                    RoleId = UserFromRequest.RoleId
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                // 6. إضافة الـ Committees
                if (UserFromRequest.CommitteeIds != null && UserFromRequest.CommitteeIds.Any())
                {
                    var committees = await _context.Committees
                        .Where(c => UserFromRequest.CommitteeIds.Contains(c.Id))
                        .ToListAsync();

                    foreach (var committee in committees)
                    {
                        user.Committees.Add(committee);
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    message = "User created successfully",
                    userId = user.Id,
                    roleName = role.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the user.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("Login")]

        public async Task<IActionResult> Login(LoginDto userFromRequest)
        {
            if (ModelState.IsValid)
            {
                User userfromdb = await userManager.FindByNameAsync(userFromRequest.Email);

                if (userfromdb != null)
                {
                    bool found = await userManager.CheckPasswordAsync(userfromdb, userFromRequest.Password);
                    if (found)
                    {
                        List<Claim> UserClaim = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userfromdb.Id.ToString()),
                    new Claim(ClaimTypes.Name, userfromdb.Email),
                    new Claim("IsActive", userfromdb.IsActive.ToString()),
                    new Claim("RoleId", userfromdb.RoleId.ToString()) 
                    }
                    ;

                        // أجيب الـ RoleName
                        var roleName = await roleManager.Roles
                            .Where(r => r.Id == userfromdb.RoleId)
                            .Select(r => r.Name)
                            .FirstOrDefaultAsync();

                        if (!string.IsNullOrEmpty(roleName))
                        {
                            UserClaim.Add(new Claim(ClaimTypes.Role, roleName));
                        }

                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            issuer: config["Jwt:IssuerIP"],
                            audience: config["Jwt:AudienceIP"],
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
                            token = tokenString,
                            user = new
                            {
                                id = userfromdb.Id,
                                firstName = userfromdb.FName,
                                middleName = userfromdb.MName,
                                lastName = userfromdb.LName,
                                email = userfromdb.Email,
                                phone = userfromdb.PhoneNumber,
                                sex = userfromdb.Sex,
                                goverment = userfromdb.Goverment,
                                year = userfromdb.Year,
                                faculty = userfromdb.Faculty,
                                roleId = userfromdb.RoleId,
                                roleName = roleName
                            }
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
