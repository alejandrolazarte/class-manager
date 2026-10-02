using System.Buffers.Text;
using System.Security.Cryptography;

namespace ClassManager.Notifications.WebPush;

public sealed record VapidKeys(string PublicKey, string PrivateKey)
{
    public static VapidKeys Generate()
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var parameters = key.ExportParameters(true);
        return new VapidKeys(
            Base64Url.EncodeToString(EcKeys.ToUncompressedPoint(parameters.Q)),
            Base64Url.EncodeToString(parameters.D));
    }
}
