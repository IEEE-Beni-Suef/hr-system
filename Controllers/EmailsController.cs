using IEEE.DTO.EmailDto;
using IEEE.Services.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IEEE.Controllers;

/// <summary>
/// API controller responsible for sending emails to one or more recipients.
/// </summary>
/// <remarks>
/// This controller is routed under <c>api/[controller]</c> and is restricted to users
/// in the "HR" role. The <see cref="SendEmail"/> action is protected by a rate limiting
/// policy named "EmailSendingPolicy".
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "HR,High Board")]
public sealed class EmailsController : ControllerBase
{
    /// <summary>
    /// Service used to dispatch emails to recipients.
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailsController"/> class.
    /// </summary>
    /// <param name="emailService">The email service used to send messages.</param>
    public EmailsController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Sends an email to the specified recipients.
    /// </summary>
    /// <remarks>
    /// The request body must include at least one recipient identifier in
    /// <see cref="SendEmailRequestDto.RecipientIds"/>. The endpoint is rate limited
    /// by the "EmailSendingPolicy". The operation is performed asynchronously and
    /// supports cancellation via <paramref name="cancellationToken"/>.
    /// </remarks>
    /// <param name="request">The DTO containing email content and recipient identifiers.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the outcome:
    /// - 200 OK with a success message and dispatched count when emails are sent.
    /// - 400 Bad Request when no recipient identifiers are provided.
    /// </returns>
    /// <response code="200">Emails dispatched successfully; response includes dispatched count.</response>
    /// <response code="400">Request did not include any recipient identifiers.</response>
    [HttpPost("send")]
    [EnableRateLimiting("EmailSendingPolicy")]
    public async Task<IActionResult> SendEmail(
        [FromBody] SendEmailRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request?.RecipientIds is null || request.RecipientIds.Count == 0)
        {
            return BadRequest(new
            {
                error = "At least one recipientId must be provided."
            });
        }
        var result = await _emailService.SendEmailAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
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

