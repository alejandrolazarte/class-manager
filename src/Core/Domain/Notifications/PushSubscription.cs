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

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid UserId { get; private set; }
    public string Endpoint { get; private set; } = string.Empty;
    public string P256dh { get; private set; } = string.Empty;
    public string Auth { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    public static Result<PushSubscription> Create(
        Guid clientId, Guid userId, string? endpoint, string? p256dh, string? auth, DateTimeOffset createdAt)
    {
        if (PushSubscriptionRules.Validate(endpoint, p256dh, auth) is { } error)
        {
            return error;
        }

        return new PushSubscription
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            UserId = userId,
            Endpoint = endpoint!,
            P256dh = p256dh!,
            Auth = auth!,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
