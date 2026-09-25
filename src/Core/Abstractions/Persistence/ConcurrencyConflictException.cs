namespace ClassManager.Core.Abstractions.Persistence;

public sealed class ConcurrencyConflictException : Exception
{
    private const string DefaultMessage = "The row was changed by someone else since it was read.";

    public ConcurrencyConflictException()
        : base(DefaultMessage)
    {
    }

    public ConcurrencyConflictException(string message)
        : base(message)
    {
    }

    public ConcurrencyConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
