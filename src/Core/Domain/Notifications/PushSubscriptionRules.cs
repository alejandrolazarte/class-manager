using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Notifications;

internal static class PushSubscriptionRules
{
    public const int EndpointMaxLength = 800;
    public const int KeyMaxLength = 128;

    private const string InvalidEndpointMessage = "The subscription endpoint must be an https address.";
    private const string InvalidKeysMessage = "The subscription keys are missing or too long.";
    private const string EndpointFieldName = "Endpoint";
    private const string KeysFieldName = "P256dh";

    public static ResultError? Validate(string? endpoint, string? p256dh, string? auth)
    {
        if (endpoint is null
            || endpoint.Length > EndpointMaxLength
            || !Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps)
        {
            return new ResultError(PushErrorCodes.InvalidEndpoint, InvalidEndpointMessage, ErrorKind.Validation)
            {
                FieldName = EndpointFieldName,
            };
        }

        return IsKey(p256dh) && IsKey(auth)
            ? null
            : new ResultError(PushErrorCodes.InvalidKeys, InvalidKeysMessage, ErrorKind.Validation)
            {
                FieldName = KeysFieldName,
            };
    }

    private static bool IsKey(string? key) => !string.IsNullOrWhiteSpace(key) && key.Length <= KeyMaxLength;
}
