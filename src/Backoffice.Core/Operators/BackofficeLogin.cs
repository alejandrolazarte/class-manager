namespace ClassManager.Backoffice.Core.Operators;

public sealed class BackofficeLogin
{
    public const int ProviderMaxLength = 40;
    public const int ProviderKeyMaxLength = 255;
    public const int EmailMaxLength = 256;

    private BackofficeLogin()
    {
    }

    public long Id { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string ProviderKey { get; private set; } = string.Empty;
    public Guid AccountId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public DateTimeOffset EnabledAt { get; private set; }
    public long? EnabledByUserId { get; private set; }

    public static BackofficeLogin Enable(ExternalIdentity identity, Guid accountId, DateTimeOffset enabledAt, long? enabledByUserId) => new()
    {
        Provider = identity.Provider,
        ProviderKey = identity.ProviderKey,
        AccountId = accountId,
        Email = identity.Email,
        EnabledAt = enabledAt,
        EnabledByUserId = enabledByUserId,
    };

    public void EnableAgain(ExternalIdentity identity, Guid accountId, DateTimeOffset enabledAt, long? enabledByUserId)
    {
        AccountId = accountId;
        Email = identity.Email;
        EnabledAt = enabledAt;
        EnabledByUserId = enabledByUserId;
    }
}
