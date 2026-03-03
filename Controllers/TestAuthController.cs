using IEEE.DTO.EmailDto.EmailAuth;
using IEEE.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IEEE.Controllers
{

    /// <summary>
    /// Controller used for testing authentication flows including password validation,
    /// generation of an email two-factor authentication (2FA) code and verification that issues a JWT.
    /// Exposes endpoints for login (password check + generate 2FA token) and verify (validate 2FA token and return JWT).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;


        public TestAuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        /// <summary>
        /// Validates the supplied email and password. If valid, generates a two-factor authentication token
        /// using the "Email" provider and sends it to the user's email. The 2FA code is NOT returned in the HTTP response.
        /// </summary>
        /// <param name="request">A <see cref="TestLoginDto"/> containing the user's Email and Password.</param>
        /// <param name="cancellationToken">Cancellation token forwarded to service operations.</param>
        /// <returns>
        /// An <see cref="IActionResult"/>:
        /// - 200 OK with an instruction message when credentials are valid;
        /// - 401 Unauthorized when the email or password is incorrect.
        /// </returns>
        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login([FromBody] TestLoginDto request, CancellationToken cancellationToken)
        {
            var result = await _authServices.Login(request, cancellationToken);
            if (!result.IsSuccess)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(new
            {
                Message = "Password verified successfully. Please check your email for the verification code."
            });
        }

        /// <summary>
        /// Verifies the two-factor authentication code previously generated for the given email.
        /// If verification succeeds, a JWT is created containing standard claims, the user's roles,
        /// and a custom "BranchId" claim (sourced from <see cref="User.CommitteeId"/>).
        /// </summary>
        /// <param name="request">A <see cref="TestVerifyDto"/> containing the Email and the 2FA Code to verify.</param>
        /// <param name="cancellationToken">Cancellation token forwarded to service operations.</param>
        /// <returns>
        /// An <see cref="IActionResult"/>:
        /// - 200 OK with a message and a signed JWT token when verification succeeds;
        /// - 400 Bad Request when the user is not found;
        /// - 401 Unauthorized when the code is invalid or has expired.
        /// </returns>
        [HttpPost("verify")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Verify([FromBody] TestVerifyDto request, CancellationToken cancellationToken)
        {
            var result = await _authServices.Verify(request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage == "User not found.")
                    return BadRequest(result.ErrorMessage);

                return Unauthorized(result.ErrorMessage);
            }

            return Ok(new
            {
                Message = "Two-factor authentication verified successfully.",
                Token = result.Value
            });
        }
    }
}