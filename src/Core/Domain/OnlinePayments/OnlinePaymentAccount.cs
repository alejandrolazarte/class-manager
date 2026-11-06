namespace ClassManager.Core.Domain.OnlinePayments;

public sealed class OnlinePaymentAccount
{
    public const int ExternalAccountIdMaxLength = 255;

    private OnlinePaymentAccount()
    {
    }

    public long Id { get; private set; }
    public long OrganizationId { get; private set; }
    public long CreatedForBusinessId { get; private set; }
    public string ExternalAccountId { get; private set; } = string.Empty;
    public bool DetailsSubmitted { get; private set; }
    public bool ChargesEnabled { get; private set; }
    public bool PayoutsEnabled { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset StatusReadAt { get; private set; }

    public OnlinePaymentStatus Status =>
        ChargesEnabled ? OnlinePaymentStatus.Active
        : DetailsSubmitted ? OnlinePaymentStatus.Restricted
        : OnlinePaymentStatus.Pending;

    public static OnlinePaymentAccount Create(long organizationId, long createdForBusinessId, string externalAccountId, DateTimeOffset now) =>
        new()
        {
            OrganizationId = organizationId,
            CreatedForBusinessId = createdForBusinessId,
            ExternalAccountId = externalAccountId,
            CreatedAt = now.ToUniversalTime(),
            StatusReadAt = now.ToUniversalTime(),
        };

    public void Update(OnlinePaymentAccountState state, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(state);
        DetailsSubmitted = state.DetailsSubmitted;
        ChargesEnabled = state.ChargesEnabled;
        PayoutsEnabled = state.PayoutsEnabled;
        StatusReadAt = now.ToUniversalTime();
    }
}
