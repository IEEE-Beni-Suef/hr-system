namespace IEEE.Services.Email.Exceptions;

public sealed class EmailDispatchException : Exception
{
    public EmailDispatchException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

