namespace ClassManager.Core.Domain.Accounts;

public sealed record LegalTermsAcceptance(string Version, DateTimeOffset AcceptedOn)
{
    public static LegalTermsAcceptance CurrentAt(DateTimeOffset acceptedOn) =>
        new(LegalTerms.CurrentVersion, acceptedOn.ToUniversalTime());

    public static bool IsCurrent(string? acceptedVersion) =>
        string.Equals(acceptedVersion, LegalTerms.CurrentVersion, StringComparison.Ordinal);
}
