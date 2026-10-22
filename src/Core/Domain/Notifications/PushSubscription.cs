using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Notifications;

public sealed class PushSubscription : ITenantOwned, IAuditable
{
    public const int EndpointMaxLength = PushSubscriptionRules.EndpointMaxLength;
    public const int KeyMaxLength = PushSubscriptionRules.KeyMaxLength;

    private PushSubscription()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClientId { get; private set; }
    public Guid UserId { get; private set; }
    public string Endpoint { get; private set; } = string.Empty;
    public string P256dh { get; private set; } = string.Empty;
    public string Auth { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    public static Result<PushSubscription> Create(
        long clientId, Guid userId, string? endpoint, string? p256dh, string? auth, DateTimeOffset createdAt)
    {
        if (PushSubscriptionRules.Validate(endpoint, p256dh, auth) is { } error)
        {
            return error;
        }

        return new PushSubscription
        {
            ClientId = clientId,
            UserId = userId,
            Endpoint = endpoint!,
            P256dh = p256dh!,
            Auth = auth!,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
