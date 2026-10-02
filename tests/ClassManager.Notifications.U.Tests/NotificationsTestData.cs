using System.Buffers.Text;
using System.Security.Cryptography;

namespace ClassManager.Notifications.U.Tests;

internal static class NotificationsTestData
{
    public static readonly DateTimeOffset Now = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);

    private const int AuthSecretLength = 16;

    public static PushTarget NewPushTarget()
    {
        using var userAgentKey = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        return new PushTarget(
            $"https://push.example.com/send/{Guid.NewGuid():N}",
            Base64Url.EncodeToString(EcKeys.ToUncompressedPoint(userAgentKey.ExportParameters(false).Q)),
            Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(AuthSecretLength)));
    }
}
