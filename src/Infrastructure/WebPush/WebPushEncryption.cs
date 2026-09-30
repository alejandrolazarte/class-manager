using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ClassManager.Infrastructure.WebPush;

internal static class WebPushEncryption
{
    public const int RecordSize = 4096;

    private const int SaltLength = 16;
    private const int KeyLength = 16;
    private const int NonceLength = 12;
    private const int TagLength = 16;
    private const int InputKeyLength = 32;
    private const byte LastRecordDelimiter = 0x02;

    private static readonly byte[] KeyInfo = Encoding.ASCII.GetBytes("Content-Encoding: aes128gcm\0");
    private static readonly byte[] NonceInfo = Encoding.ASCII.GetBytes("Content-Encoding: nonce\0");
    private static readonly byte[] WebPushInfo = Encoding.ASCII.GetBytes("WebPush: info\0");

    public static byte[] Encrypt(byte[] plaintext, byte[] userAgentPublicKey, byte[] authSecret)
    {
        using var applicationServerKey = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        return Encrypt(plaintext, userAgentPublicKey, authSecret, applicationServerKey, RandomNumberGenerator.GetBytes(SaltLength));
    }

    public static byte[] Encrypt(
        byte[] plaintext,
        byte[] userAgentPublicKey,
        byte[] authSecret,
        ECDiffieHellman applicationServerKey,
        byte[] salt)
    {
        var applicationServerPublicKey = EcKeys.ToUncompressedPoint(applicationServerKey.ExportParameters(false).Q);
        using var userAgentKey = EcKeys.ImportDiffieHellmanPublicKey(userAgentPublicKey);
        var sharedSecret = applicationServerKey.DeriveRawSecretAgreement(userAgentKey);
        var inputKey = HKDF.DeriveKey(
            HashAlgorithmName.SHA256, sharedSecret, InputKeyLength, authSecret, [.. WebPushInfo, .. userAgentPublicKey, .. applicationServerPublicKey]);
        var contentKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, inputKey, KeyLength, salt, KeyInfo);
        var nonce = HKDF.DeriveKey(HashAlgorithmName.SHA256, inputKey, NonceLength, salt, NonceInfo);

        byte[] record = [.. plaintext, LastRecordDelimiter];
        var ciphertext = new byte[record.Length];
        var tag = new byte[TagLength];
        using (var aes = new AesGcm(contentKey, TagLength))
        {
            aes.Encrypt(nonce, record, ciphertext, tag);
        }

        var recordSize = new byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(recordSize, RecordSize);
        return [.. salt, .. recordSize, (byte)applicationServerPublicKey.Length, .. applicationServerPublicKey, .. ciphertext, .. tag];
    }
}
