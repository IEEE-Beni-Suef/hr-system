//using IEEE.Data;
//using IEEEApplication.Results;
//using IEEE.DTO.EmailDto;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.Extensions.Logging;

//namespace IEEE.Services.Emails
//{
//    public sealed class EmailService : IEmailService
//    {
//        private readonly AppDbContext _dbContext;
//        private readonly IEEE.Services.EmailSettings.EmailSettings _settings;
//        private readonly ILogger<EmailService> _logger;

//        public EmailService(
//            AppDbContext dbContext,
//            IOptions<IEEE.Services.EmailSettings.EmailSettings> options,
//            ILogger<EmailService> logger)
//        {
//            _dbContext = dbContext;
//            _settings = options.Value;
//            _logger = logger;
//        }

//        public async Task<Result<int>> SendEmailAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default)
//        {
//            if (request is null)
//                return Result<int>.Failure("Request object is null");

//            if (request.RecipientIds is null || !request.RecipientIds.Any())
//                return Result<int>.Failure("RecipientIds is empty");

//            var recipientEmails = await _dbContext.Set<IEEE.Entities.User>()
//                .Where(u => request.RecipientIds.Contains(u.Id))
//                .Select(u => u.Email)
//                .Where(e => !string.IsNullOrWhiteSpace(e))
//                .Distinct()
//                .ToListAsync(cancellationToken);

//            if (!recipientEmails.Any())
//                return Result<int>.Failure("No valid recipients found");

//            var message = BuildMimeMessage(request, recipientEmails);

//            try
//            {
//                _logger.LogInformation("Connecting to SMTP server {Server}:{Port}", _settings.SmtpServer, _settings.Port);

//                using var smtpClient = new SmtpClient();
//                // جرب Auto بدل StartTls
//                await smtpClient.ConnectAsync(
//                    _settings.SmtpServer,
//                    _settings.Port,
//                    SecureSocketOptions.Auto,  // ← خلي MailKit يحدد لوحده
//                    cancellationToken);

//                _logger.LogInformation("Authenticating as {SenderEmail}", _settings.SenderEmail);
//                await smtpClient.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword, cancellationToken);

//                _logger.LogInformation("Sending email to {Count} recipients", recipientEmails.Count);
//                await smtpClient.SendAsync(message, cancellationToken);
//                await smtpClient.DisconnectAsync(true, cancellationToken);

//                _logger.LogInformation("Email successfully sent to {Count} recipients", recipientEmails.Count);

//                return Result<int>.Success(recipientEmails.Count);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "SMTP dispatch failed for {Count} recipients", recipientEmails.Count);
//                return Result<int>.Failure($"SMTP Error: {ex.Message} | Inner: {ex.InnerException?.Message}");
//            }
//        }

//        private MimeMessage BuildMimeMessage(SendEmailRequestDto request, IReadOnlyCollection<string> recipientEmails)
//        {
//            var message = new MimeMessage();
//            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

//            foreach (var email in recipientEmails)
//                message.Bcc.Add(MailboxAddress.Parse(email));

//            message.Subject = request.Subject;
//            message.Body = new BodyBuilder { HtmlBody = request.Body }.ToMessageBody();
//            return message;
//        }
//    }
//}



using IEEE.Data;
using IEEEApplication.Results;
using IEEE.DTO.EmailDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace IEEE.Services.Emails
{
    public sealed class EmailService : IEmailService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEEE.Services.EmailSettings.EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            AppDbContext dbContext,
            IOptions<IEEE.Services.EmailSettings.EmailSettings> options,
            ILogger<EmailService> logger)
        {
            _dbContext = dbContext;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Result<int>> SendEmailAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request is null)
                return Result<int>.Failure("Request object is null");

            if (request.RecipientIds is null || !request.RecipientIds.Any())
                return Result<int>.Failure("RecipientIds is empty");

            var recipientEmails = await _dbContext.Set<IEEE.Entities.User>()
                .Where(u => request.RecipientIds.Contains(u.Id))
                .Select(u => u.Email)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!recipientEmails.Any())
                return Result<int>.Failure("No valid recipients found");

            try
            {
                using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,  // ← مهم
                    Timeout = 20000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(_settings.SenderEmail));

                foreach (var email in recipientEmails)
                    mailMessage.Bcc.Add(new MailAddress(email));

                await client.SendMailAsync(mailMessage, cancellationToken);

                _logger.LogInformation("Email sent to {Count} recipients", recipientEmails.Count);
                return Result<int>.Success(recipientEmails.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email dispatch failed");
                return Result<int>.Failure($"Error: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }
    }
}