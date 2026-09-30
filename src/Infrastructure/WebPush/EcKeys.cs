using System.Buffers.Text;
using System.Security.Cryptography;

namespace ClassManager.Infrastructure.WebPush;

internal static class EcKeys
{
    public const int UncompressedPointLength = 65;

    private const byte UncompressedPointPrefix = 0x04;
    private const int CoordinateLength = 32;

    public static byte[] ToUncompressedPoint(ECPoint point) =>
        [UncompressedPointPrefix, .. point.X!, .. point.Y!];

    public static ECPoint FromUncompressedPoint(ReadOnlySpan<byte> publicKey)
    {
        if (publicKey.Length != UncompressedPointLength || publicKey[0] != UncompressedPointPrefix)
        {
            throw new CryptographicException("The public key is not an uncompressed P-256 point.");
        }

        return new ECPoint
        {
            X = publicKey.Slice(1, CoordinateLength).ToArray(),
            Y = publicKey.Slice(1 + CoordinateLength, CoordinateLength).ToArray(),
        };
    }

    public static ECDiffieHellmanPublicKey ImportDiffieHellmanPublicKey(ReadOnlySpan<byte> publicKey)
    {
        using var key = ECDiffieHellman.Create(new ECParameters { Curve = ECCurve.NamedCurves.nistP256, Q = FromUncompressedPoint(publicKey) });
        return key.PublicKey;
    }

    public static ECDiffieHellman ImportDiffieHellman(string privateKey, string publicKey) =>
        ECDiffieHellman.Create(Parameters(privateKey, publicKey));

    public static ECDsa ImportSigningKey(string privateKey, string publicKey) =>
        ECDsa.Create(Parameters(privateKey, publicKey));

    public static ECDsa ImportSigningPublicKey(string publicKey) =>
        ECDsa.Create(new ECParameters { Curve = ECCurve.NamedCurves.nistP256, Q = FromUncompressedPoint(Base64Url.DecodeFromChars(publicKey)) });

    private static ECParameters Parameters(string privateKey, string publicKey) =>
        new()
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = Base64Url.DecodeFromChars(privateKey),
            Q = FromUncompressedPoint(Base64Url.DecodeFromChars(publicKey)),
        };
}
