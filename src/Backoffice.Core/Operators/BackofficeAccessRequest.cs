namespace ClassManager.Backoffice.Core.Operators;

public sealed class BackofficeAccessRequest
{
    public const int CodeMaxLength = 6;

    private BackofficeAccessRequest()
    {
    }

    public Guid Id { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string ProviderKey { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; private set; }

    public static BackofficeAccessRequest Create(ExternalIdentity identity, string code, DateTimeOffset requestedAt) => new()
    {
        Id = Guid.CreateVersion7(),
        Provider = identity.Provider,
        ProviderKey = identity.ProviderKey,
        Email = identity.Email,
        Code = code,
        RequestedAt = requestedAt,
    };

    public ExternalIdentity ToIdentity() => new(Provider, ProviderKey, Email);

    public void Renew(ExternalIdentity identity, string code, DateTimeOffset requestedAt)
    {
        Email = identity.Email;
        Code = code;
        RequestedAt = requestedAt;
    }
}
