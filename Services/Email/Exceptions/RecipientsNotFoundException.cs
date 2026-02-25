namespace IEEE.Services.Email.Exceptions;
public sealed class RecipientsNotFoundException : Exception
{
    public IReadOnlyCollection<int> RecipientIds { get; }

    public RecipientsNotFoundException(IReadOnlyCollection<int> recipientIds)
        : base("No valid recipients were found for the provided IDs.")
    {
        RecipientIds = recipientIds;
    }
}

