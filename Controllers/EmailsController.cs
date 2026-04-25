using IEEE.DTO.EmailDto;
using IEEE.Services.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IEEE.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "HR,High Board")]
public sealed class EmailsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailsController> _logger;

    public EmailsController(IEmailService emailService, ILogger<EmailsController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("send")]
    [EnableRateLimiting("EmailSendingPolicy")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDto request, CancellationToken cancellationToken)
    {
        if (request?.RecipientIds is null || !request.RecipientIds.Any())
            return BadRequest(new { error = "At least one RecipientId must be provided." });

        var result = await _emailService.SendEmailAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Email sending failed: {Error}", result.ErrorMessage);
            return BadRequest(new
            {
                error = result.ErrorMessage
            });
        }

        return Ok(new
        {
            status = "Success",
            message = $"Emails dispatched successfully to {result.Value} recipients."
        });
    }
}