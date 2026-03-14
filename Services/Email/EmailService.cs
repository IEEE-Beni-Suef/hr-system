using IEEE.Data;
using IEEE.DTO.EmailDto;
using IEEE.Services.Email.Exceptions;
using IEEE.Services.Emails;
using IEEE.Services.OptionsPatterns;
using IEEEApplication.Results;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace IEEE.Services.Email;

public sealed class EmailService : IEmailService
{
    private readonly AppDbContext _dbContext;
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        AppDbContext dbContext,
        IOptions<EmailSettings> options,
        ILogger<EmailService> logger)
    {
        _dbContext = dbContext;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<Result<int>> SendEmailAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            return Result<int>.Failure($"The {nameof(request)} is null");

        if (request.RecipientIds is null || request.RecipientIds.Count == 0)
            return Result<int>.Failure($"The {nameof(request.RecipientIds)} is Empty");

        var recipientEmails = await _dbContext.Set<IEEE.Entities.User>()
            .Where(u => request.RecipientIds.Contains(u.Id))
            .Select(u => u.Email)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToListAsync(cancellationToken);

        var nonNullRecipientEmails = recipientEmails
            .Where(e => e is not null)
            .Cast<string>()
            .ToList();

        if (nonNullRecipientEmails.Count == 0)
            return Result<int>.Failure("No valid recipient emails found.");

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            // نرسل الإيميل لأنفسنا في الـ To ونحط الأعضاء في BCC
            // عشان الأعضاء ميشوفوش إيميلات بعضهم
            message.To.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            foreach (var email in nonNullRecipientEmails)
                message.Bcc.Add(MailboxAddress.Parse(email));

            message.Subject = request.Subject;

            // بنبعت HTML template
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = BuildEmailTemplate(request.Subject, request.Body)
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword, cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to dispatch email to {RecipientCount} recipients via Gmail SMTP.",
                nonNullRecipientEmails.Count);

            throw new EmailDispatchException(
                "An error occurred while dispatching the emails via Gmail SMTP.",
                ex);
        }

        return Result<int>.Success(nonNullRecipientEmails.Count);
    }

    private static string BuildEmailTemplate(string subject, string body)
    {
        return $"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8"/>
      <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
      <title>{subject}</title>
    </head>
    <body style="margin:0;padding:0;background-color:#eef0f8;font-family:'Segoe UI',Arial,sans-serif;">

      <table width="100%" cellpadding="0" cellspacing="0" style="background:#eef0f8;padding:40px 0;">
        <tr>
          <td align="center">
            <table width="600" cellpadding="0" cellspacing="0"
              style="max-width:600px;width:100%;background:#ffffff;border-radius:10px;overflow:hidden;">

              <!-- ===== HEADER ===== -->
              <tr>
                <td style="background:#1a237e;padding:32px 40px 22px;text-align:center;">
                  <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                      <td align="center">
                        <!-- IEEE diamond icon (SVG inline) -->
                        <table cellpadding="0" cellspacing="0" style="margin:0 auto 10px;">
                          <tr>
                            <td style="padding-right:14px;vertical-align:middle;">
                              <svg width="44" height="44" viewBox="0 0 44 44" xmlns="http://www.w3.org/2000/svg">
                                <rect x="1" y="1" width="42" height="42" rx="4"
                                  fill="none" stroke="rgba(255,255,255,0.4)" stroke-width="1.5"/>
                                <rect x="13" y="13" width="18" height="18" rx="2"
                                  fill="none" stroke="white" stroke-width="1.5"/>
                                <circle cx="22" cy="22" r="4" fill="white"/>
                                <line x1="1" y1="22" x2="13" y2="22"
                                  stroke="white" stroke-width="1.5"/>
                                <line x1="31" y1="22" x2="43" y2="22"
                                  stroke="white" stroke-width="1.5"/>
                                <line x1="22" y1="1" x2="22" y2="13"
                                  stroke="white" stroke-width="1.5"/>
                                <line x1="22" y1="31" x2="22" y2="43"
                                  stroke="white" stroke-width="1.5"/>
                              </svg>
                            </td>
                            <td style="vertical-align:middle;text-align:left;">
                              <span style="display:block;font-size:30px;font-weight:900;
                                color:#ffffff;letter-spacing:5px;line-height:1;">IEEE</span>
                              <span style="display:block;font-size:10px;
                                color:rgba(255,255,255,0.65);letter-spacing:2px;margin-top:3px;">
                                BENI SUEF STUDENT BRANCH
                              </span>
                            </td>
                          </tr>
                        </table>
                        <!-- Wheat-inspired decoration -->
                        <div style="font-size:18px;color:rgba(255,255,255,0.4);
                          letter-spacing:4px;margin-top:6px;">
                          &#10038; &#10038; &#10038;
                        </div>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>

              <!-- Gradient accent bar -->
              <tr>
                <td style="background:linear-gradient(90deg,#1a237e,#3949ab,#7986cb);
                  height:4px;font-size:0;line-height:0;">&nbsp;</td>
              </tr>

              <!-- ===== SUBJECT BANNER ===== -->
              <tr>
                <td style="background:#f5f7ff;padding:20px 36px;
                  border-bottom:1px solid #e0e4f0;">
                  <p style="margin:0 0 5px;font-size:11px;color:#3949ab;
                    font-weight:600;letter-spacing:1.5px;text-transform:uppercase;">
                    New Message
                  </p>
                  <h1 style="margin:0;font-size:20px;font-weight:700;color:#1a237e;">
                    {subject}
                  </h1>
                </td>
              </tr>

              <!-- ===== BODY ===== -->
              <tr>
                <td style="padding:32px 36px;">
                  <div style="font-size:15px;line-height:1.8;color:#37474f;">
                    {body}
                  </div>
                </td>
              </tr>

              <!-- Divider -->
              <tr>
                <td style="padding:0 36px;">
                  <div style="height:1px;background:#e0e4f0;font-size:0;">&nbsp;</div>
                </td>
              </tr>

              <!-- ===== FOOTER ===== -->
              <tr>
                <td style="background:#f5f7ff;padding:22px 36px;text-align:center;">
                  <p style="margin:0 0 6px;font-size:12px;font-weight:700;
                    color:#1a237e;letter-spacing:1px;">
                    IEEE BENI SUEF STUDENT BRANCH
                  </p>
                  <p style="margin:0;font-size:11px;color:#90a4ae;line-height:1.7;">
                    This is an automated message sent by the IEEE Branch management system.<br/>
                    Please do not reply directly to this email.
                  </p>
                  <p style="margin:14px 0 0;font-size:20px;
                    color:rgba(26,35,126,0.3);letter-spacing:6px;">
                    &#9670; &#9670; &#9670;
                  </p>
                </td>
              </tr>

              <!-- Bottom solid bar -->
              <tr>
                <td style="background:#1a237e;height:5px;
                  font-size:0;line-height:0;border-radius:0 0 10px 10px;">&nbsp;</td>
              </tr>

            </table>
          </td>
        </tr>
      </table>

    </body>
    </html>
    """;
    }
}