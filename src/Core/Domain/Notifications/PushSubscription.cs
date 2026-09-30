using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Notifications;

public sealed class PushSubscription : ITenantOwned
{
    public const int EndpointMaxLength = 800;
    public const int KeyMaxLength = 128;

    private const string InvalidEndpointMessage = "The subscription endpoint must be an https address.";
    private const string InvalidKeysMessage = "The subscription keys are missing or too long.";

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

    public static Result<PushSubscription> Create(
        Guid clientId, Guid userId, string? endpoint, string? p256dh, string? auth, DateTimeOffset createdAt)
    {
        if (endpoint is null
            || endpoint.Length > EndpointMaxLength
            || !Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps)
        {
            return Result.Validation<PushSubscription>(InvalidEndpointMessage, PushErrorCodes.InvalidEndpoint, nameof(Endpoint));
        }

        if (!IsKey(p256dh) || !IsKey(auth))
        {
            return Result.Validation<PushSubscription>(InvalidKeysMessage, PushErrorCodes.InvalidKeys, nameof(P256dh));
        }

        return new PushSubscription
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            UserId = userId,
            Endpoint = endpoint,
            P256dh = p256dh!,
            Auth = auth!,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    private static bool IsKey(string? key) => !string.IsNullOrWhiteSpace(key) && key.Length <= KeyMaxLength;
}
