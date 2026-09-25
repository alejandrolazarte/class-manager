namespace ClassManager.Core.Abstractions.Persistence;

public sealed class UniqueConstraintViolationException : Exception
{
    private const string DefaultMessage = "A unique constraint was violated while saving changes.";

    public UniqueConstraintViolationException()
        : base(DefaultMessage)
    {
    }

    public UniqueConstraintViolationException(string message)
        : base(message)
    {
    }

    public UniqueConstraintViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
