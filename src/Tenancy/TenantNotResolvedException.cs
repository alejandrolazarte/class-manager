namespace ClassManager.Tenancy;

public sealed class TenantNotResolvedException : Exception
{
    private const string DefaultMessage = "The current tenant could not be resolved.";

    public TenantNotResolvedException()
        : base(DefaultMessage)
    {
    }

    public TenantNotResolvedException(string message)
        : base(message)
    {
    }

    public TenantNotResolvedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
