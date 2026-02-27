using IEEE.DTO.EmailDto.EmailAuth;
using IEEEApplication.Results;
using IEEE.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IEEE.DTO.EmailDto;
using IEEE.Services.Emails;

namespace IEEE.Services.Auth
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthServices(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<Result<string>> Login(TestLoginDto login, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Result<string>.Failure("Email or password is incorrect.");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var emailRequest = new SendEmailRequestDto
            {
                Subject = "Your verification code (2FA) - IEEE System",
                Body = $@"
                    <div style='font-family: Arial;'>
                        <h3>Hello {user.FName},</h3>
                        <p>You have requested to sign in to the IEEE system.</p>
                        <p>Your verification code is: <strong style='font-size: 20px; color: blue;'>{code}</strong></p>
                        <p>This code is valid for a short time. Please do not share it with anyone.</p>
                    </div>",
                RecipientIds = new List<int> { user.Id }
            };

            await _emailService.SendEmailAsync(emailRequest, cancellationToken);

            return Result<string>.Success("Please check your email for the verification code.");
        }

        public async Task<Result<string>> Verify(TestVerifyDto verify, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(verify.Email);
            if (user == null)
            {
                return Result<string>.Failure("User not found.");
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", verify.Code);
            if (!isValid)
            {
                return Result<string>.Failure("Code is invalid or has expired.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            if (user.CommitteeId.HasValue)
            {
                claims.Add(new Claim("BranchId", user.CommitteeId.Value.ToString()));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSection = _configuration.GetSection("Jwt");

            var keyValue = jwtSection["SecritKey"];
            var issuer = jwtSection["IssuerIP"];
            var audience = jwtSection["AudienceIP"];

            if (string.IsNullOrWhiteSpace(keyValue) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return Result<string>.Failure("JWT configuration is invalid.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), 
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Result<string>.Success(tokenString);
        }
    }
}
