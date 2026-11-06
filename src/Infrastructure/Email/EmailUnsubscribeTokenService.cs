using System.Buffers.Text;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Security.Tokens;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.Email;

internal sealed class EmailUnsubscribeTokenService(IOptions<JwtOptions> jwtOptions) : IEmailUnsubscribeTokenService
{
    private const string Purpose = "email-unsubscribe-v1";
    private const char PartSeparator = '.';
    private const char FieldSeparator = '|';
    private const int FieldCount = 3;

    public string Create(EmailUnsubscription unsubscription)
    {
        var payload = Encoding.UTF8.GetBytes(string.Join(
            FieldSeparator,
            unsubscription.BusinessId.ToString(CultureInfo.InvariantCulture),
            unsubscription.AccountId.ToString("N"),
            unsubscription.Category.ToString()));
        return Base64Url.EncodeToString(payload) + PartSeparator + Base64Url.EncodeToString(Sign(payload));
    }

    public EmailUnsubscription? Read(string? token)
    {
        var parts = token?.Split(PartSeparator);
        if (parts is not { Length: 2 } || !TryDecode(parts[0], out var payload) || !TryDecode(parts[1], out var signature)
            || !CryptographicOperations.FixedTimeEquals(Sign(payload), signature))
        {
            return null;
        }

        var fields = Encoding.UTF8.GetString(payload).Split(FieldSeparator);
        return fields.Length == FieldCount
            && long.TryParse(fields[0], NumberStyles.None, CultureInfo.InvariantCulture, out var businessId)
            && Guid.TryParseExact(fields[1], "N", out var accountId)
            && Enum.TryParse<NotificationCategory>(fields[2], ignoreCase: false, out var category)
            && Enum.IsDefined(category)
                ? new EmailUnsubscription(businessId, accountId, category)
                : null;
    }

    private byte[] Sign(byte[] payload)
    {
        var purposeKey = HMACSHA256.HashData(Encoding.UTF8.GetBytes(jwtOptions.Value.SigningKey), Encoding.UTF8.GetBytes(Purpose));
        return HMACSHA256.HashData(purposeKey, payload);
    }

    private static bool TryDecode(string text, out byte[] bytes)
    {
        try
        {
            bytes = Base64Url.DecodeFromChars(text);
            return true;
        }
        catch (FormatException)
        {
            bytes = [];
            return false;
        }
    }
}
