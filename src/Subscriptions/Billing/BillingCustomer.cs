namespace ClassManager.Subscriptions.Billing;

public sealed class BillingCustomer
{
    public const int ExternalCustomerIdMaxLength = 255;

    private BillingCustomer()
    {
    }

    public long SubscriberId { get; private set; }
    public string ExternalCustomerId { get; private set; } = string.Empty;

    public static BillingCustomer Create(long subscriberId, string externalCustomerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalCustomerId);
        if (externalCustomerId.Length > ExternalCustomerIdMaxLength)
        {
            throw new ArgumentException(
                $"External customer id must be at most {ExternalCustomerIdMaxLength} characters.",
                nameof(externalCustomerId));
        }

        return new BillingCustomer
        {
            SubscriberId = subscriberId,
            ExternalCustomerId = externalCustomerId,
        };
    }
}
