using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ClassManager.Infrastructure.WebPush;

internal static class VapidAuthorization
{
    public static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(12);

    private static readonly string EncodedHeader =
        Base64Url.EncodeToString(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, string> { ["typ"] = "JWT", ["alg"] = "ES256" }));

    public static string Create(VapidOptions options, Uri endpoint, DateTimeOffset now)
    {
        var claims = new Dictionary<string, object>
        {
            ["aud"] = endpoint.GetLeftPart(UriPartial.Authority),
            ["exp"] = (now + TokenLifetime).ToUnixTimeSeconds(),
            ["sub"] = options.Subject,
        };
        var unsignedToken = $"{EncodedHeader}.{Base64Url.EncodeToString(JsonSerializer.SerializeToUtf8Bytes(claims))}";
        using var key = EcKeys.ImportSigningKey(options.PrivateKey, options.PublicKey);
        var signature = key.SignData(Encoding.ASCII.GetBytes(unsignedToken), HashAlgorithmName.SHA256);
        return $"vapid t={unsignedToken}.{Base64Url.EncodeToString(signature)}, k={options.PublicKey}";
    }
}
