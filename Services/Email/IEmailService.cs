using IEEE.DTO.EmailDto;
using IEEEApplication.Results;

namespace IEEE.Services.Emails;

public interface IEmailService
{
    Task<Result<int>> SendEmailAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default);
}


