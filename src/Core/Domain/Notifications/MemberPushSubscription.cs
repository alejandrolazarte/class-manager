using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Notifications;

public sealed class MemberPushSubscription : ITenantOwned
{
    public const int EndpointMaxLength = PushSubscriptionRules.EndpointMaxLength;
    public const int KeyMaxLength = PushSubscriptionRules.KeyMaxLength;

    private MemberPushSubscription()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string Endpoint { get; private set; } = string.Empty;
    public string P256dh { get; private set; } = string.Empty;
    public string Auth { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<MemberPushSubscription> Create(
        Guid userId, string? endpoint, string? p256dh, string? auth, DateTimeOffset createdAt)
    {
        if (PushSubscriptionRules.Validate(endpoint, p256dh, auth) is { } error)
        {
            return error;
        }

        return new MemberPushSubscription
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Endpoint = endpoint!,
            P256dh = p256dh!,
            Auth = auth!,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
