namespace ClassManager.Core.Domain.Accounts;

public sealed class LegalTermsAcceptance
{
    private LegalTermsAcceptance()
    {
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Version { get; private set; } = string.Empty;
    public DateTimeOffset AcceptedOn { get; private set; }

    public static LegalTermsAcceptance Record(Guid userId, string version, DateTimeOffset acceptedOn) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Version = version,
            AcceptedOn = acceptedOn.ToUniversalTime(),
        };

    public static LegalTermsAcceptance RecordCurrent(Guid userId, DateTimeOffset acceptedOn) =>
        Record(userId, LegalTerms.CurrentVersion, acceptedOn);
}
