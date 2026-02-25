using IEEE.Data;
using IEEEApplication.Results;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IEEE.Services.EmailSettings;
using MimeKit;
using IEEE.DTO.EmailDto;
using IEEE.Services.Email.Exceptions;
using IEEE.Services.Emails;

namespace IEEE.Services.Email;

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
        {
            return Result<int>.Failure($"The {nameof(request)} is null");
        }

        if (request.RecipientIds is null || request.RecipientIds.Count == 0)
        {
            return Result<int>.Failure($"The {nameof(request.RecipientIds)} is Empty");
        }

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
        {
            return Result<int>.Failure($"The {nameof(request.RecipientIds)} is Empty");
        }

        var message = BuildMimeMessage(request, nonNullRecipientEmails);

        try
        {
            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.Auto,
                cancellationToken);

            await smtpClient.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.AppPassword,
                cancellationToken);

            await smtpClient.SendAsync(message, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to dispatch email to {RecipientCount} recipients.",
                nonNullRecipientEmails.Count);

            throw new EmailDispatchException(
                "An error occurred while dispatching the emails.",
                ex);
        }

        return Result<int>.Success(nonNullRecipientEmails.Count);
    }

    private MimeMessage BuildMimeMessage(SendEmailRequestDto request, IReadOnlyCollection<string> recipientEmails)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

        foreach (var email in recipientEmails)
        {
            message.Bcc.Add(MailboxAddress.Parse(email));
        }

        message.Subject = request.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = request.Body
        };

        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }
}

