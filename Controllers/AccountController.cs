using IEEE.Data;
using IEEE.DTO.UserDTO;
using IEEE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 1. تحقق من وجود الـ Role بالـ Id
                var role = await roleManager.FindByIdAsync(dto.RoleId.ToString());
                if (role == null)
                    return BadRequest(new { message = $"Role with ID '{dto.RoleId}' does not exist." });

                // نخزن الاسم جوه الـ DTO عشان نستخدمه مع Identity
                dto.RoleName = role.Name;

                // 2. منع تكرار الإيميل
                var existingUser = await userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    return BadRequest(new { message = "Email already exists." });

                // 3. إنشاء كيان الـ User
                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FName = dto.FirstName,
                    MName = dto.MiddleName,
                    LName = dto.LastName,
                    Faculty = dto.Faculty,
                    PhoneNumber = dto.Phone,
                    Sex = dto.Sex,
                    Goverment = dto.Goverment,
                    Year = dto.Year,
                    IsActive = false,
                    EmailConfirmed = false,
                    RoleId = dto.RoleId // نخزن الـ RoleId في جدول Users
                };

                // 4. إنشاء المستخدم في Identity
                var createResult = await userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description).ToArray();
                    return BadRequest(new { step = "CreateUser", errors });
                }

                // 5. إضافة الـ Role باستخدام الاسم
                var addRoleResult = await userManager.AddToRoleAsync(user, dto.RoleName);
                if (!addRoleResult.Succeeded)
                {
                    await userManager.DeleteAsync(user); // Cleanup لو فشل
                    var errors = addRoleResult.Errors.Select(e => e.Description).ToArray();
                    return BadRequest(new { step = "AddToRole", errors });
                }

                // 6. جلب المستخدم من الـ DbContext لتحديث الـ Committees
                var userFromDb = await _context.Users
                    .Include(u => u.Committees)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (userFromDb == null)
                    return StatusCode(500, new { message = "User created but could not be reloaded." });

                // 7. إضافة Committees (لو فيه)
                if (dto.CommitteeIds != null && dto.CommitteeIds.Any())
                {
                    var committees = await _context.Committees
                        .Where(c => dto.CommitteeIds.Contains(c.Id))
                        .ToListAsync();

                    foreach (var c in committees)
                    {
                        if (!userFromDb.Committees.Any(x => x.Id == c.Id))
                            userFromDb.Committees.Add(c);
                    }

                    await _context.SaveChangesAsync();
                }

                // 8. جلب الـ Roles الفعلية للمستخدم (Debug أو للتأكيد)
                var actualRoles = await userManager.GetRolesAsync(user);

                return Ok(new
                {
                    message = "User created successfully",
                    userId = user.Id,
                    roleName = dto.RoleName,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the user.",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }

        private string GenerateJwtToken(User user, IList<string> userRoles)
        {
            List<Claim> UserClaim = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("IsActive", user.IsActive.ToString()),
            new Claim("RoleId", user.RoleId.ToString())
        };

            foreach (var roleName in userRoles)
            {
                UserClaim.Add(new Claim(ClaimTypes.Role, roleName));
            }

            // Use configured signing key and expiration
            var signingKey = config["Jwt:SecritKey"] ?? "KiraSuperUltraMegaSecretKey!1234567890";
            var expirationMinutes = 180;
            if (int.TryParse(config["Jwt:AccessTokenExpirationMinutes"], out var parsedMinutes))
            {
                expirationMinutes = parsedMinutes;
            }

            var token = new JwtSecurityToken(
                issuer: config["Jwt:IssuerIP"],
                audience: config["Jwt:AudienceIP"],
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                claims: UserClaim,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(dto.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Username or password invalid");

            var roles = await userManager.GetRolesAsync(user);

            // Access Token
            var accessToken = GenerateJwtToken(user, roles);

            // Refresh Token
            var refreshToken = GenerateRefreshToken(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);

            return Ok(new
            {
                accessToken,
                refreshToken = refreshToken.Token,
                user = new
                {
                    id = user.Id,
                    firstName = user.FName,
                    middleName = user.MName,
                    lastName = user.LName,
                    email = user.Email,
                    phone = user.PhoneNumber,
                    sex = user.Sex,
                    goverment = user.Goverment,
                    year = user.Year,
                    faculty = user.Faculty,
                    roleId = user.RoleId,
                    roleName = roles.FirstOrDefault(),
                    roles
                }
            });
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7), // مدة صلاحية الـ Refresh Token
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress ?? "unknown",
                // ensure DB non-nullable columns receive a non-null value
                RevokedByIp = string.Empty,
                ReplacedByToken = string.Empty
            };
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return Unauthorized();

            var oldToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!oldToken.IsActive)
                return Unauthorized("Invalid refresh token");

            var roles = await userManager.GetRolesAsync(user);

            // Access Token جديد
            var newJwt = GenerateJwtToken(user, roles);
            var newRefreshToken = GenerateRefreshToken(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

            // إلغاء القديم وربطه بالجديد
            oldToken.Revoked = DateTime.UtcNow;
            oldToken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            oldToken.ReplacedByToken = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newJwt,
                refreshToken = newRefreshToken.Token
            });
        }
    }
}