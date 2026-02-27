namespace IEEE.DTO.EmailDto;

public sealed class SendEmailRequestDto
{
    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public IReadOnlyCollection<int> RecipientIds { get; init; } = Array.Empty<int>();
}

