namespace ClassManager.Core.Domain.Accounts;

public sealed class PendingSignUp
{
    public const int TokenHashLength = 64;

    public static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

    private PendingSignUp()
    {
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string OwnerFullName { get; private set; } = string.Empty;
    public DateOnly OwnerBirthDate { get; private set; }
    public string BusinessName { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public string TimeZoneId { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? AcceptedLegalTermsVersion { get; private set; }
    public DateTimeOffset? AcceptedLegalTermsOn { get; private set; }

    public static PendingSignUp Start(
        OwnerProfile owner,
        PendingBusiness business,
        string tokenHash,
        DateTimeOffset createdAt,
        string acceptedLegalTermsVersion)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(business);

        return new PendingSignUp
        {
            Id = Guid.CreateVersion7(),
            Email = owner.Email,
            OwnerFullName = owner.FullName,
            OwnerBirthDate = owner.BirthDate,
            BusinessName = business.Name,
            CountryCode = business.CountryCode,
            TimeZoneId = business.TimeZoneId,
            CurrencyCode = business.CurrencyCode,
            TokenHash = tokenHash,
            CreatedAt = createdAt.ToUniversalTime(),
            ExpiresAt = (createdAt + Lifetime).ToUniversalTime(),
            AcceptedLegalTermsVersion = acceptedLegalTermsVersion,
            AcceptedLegalTermsOn = createdAt.ToUniversalTime(),
        };
    }

    public PendingBusiness Business => new(BusinessName, CountryCode, TimeZoneId, CurrencyCode);

    public LegalTermsAcceptance? LegalTermsAcceptanceOf(Guid userId) =>
        AcceptedLegalTermsVersion is { } version && AcceptedLegalTermsOn is { } acceptedOn
            ? LegalTermsAcceptance.Record(userId, version, acceptedOn)
            : null;

    public bool IsPendingAt(DateTimeOffset now) => CompletedAt is null && RevokedAt is null && ExpiresAt > now;

    public void Complete(DateTimeOffset completedAt) => CompletedAt = completedAt.ToUniversalTime();

    public void Revoke(DateTimeOffset revokedAt) => RevokedAt = revokedAt.ToUniversalTime();
}
