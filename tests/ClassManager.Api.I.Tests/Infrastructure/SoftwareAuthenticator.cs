using System.Buffers.Binary;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClassManager.Api.I.Tests.Infrastructure;

/// <summary>
/// A platform authenticator in memory: creates ES256 passkeys and signs WebAuthn challenges the way a browser and a
/// device would, so the API runs Identity's real attestation and assertion checks.
/// </summary>
public sealed class SoftwareAuthenticator : IDisposable
{
    public const string AllowedOrigin = "http://localhost:8081";
    public const string RelyingPartyId = "localhost";

    private const string CreateCeremonyType = "webauthn.create";
    private const string GetCeremonyType = "webauthn.get";
    private const string PublicKeyCredentialType = "public-key";
    private const string NoneAttestationFormat = "none";
    private const string PlatformAttachment = "platform";
    private const string InternalTransport = "internal";
    private const int CredentialIdByteCount = 32;
    private const int AuthenticatorGuidByteCount = 16;
    private const byte UserPresentFlag = 0x01;
    private const byte UserVerifiedFlag = 0x04;
    private const byte AttestedCredentialDataFlag = 0x40;
    private const int CoseKeyTypeLabel = 1;
    private const int CoseAlgorithmLabel = 3;
    private const int CoseCurveLabel = -1;
    private const int CoseXLabel = -2;
    private const int CoseYLabel = -3;
    private const int CoseEllipticCurveKeyType = 2;
    private const int CoseEs256Algorithm = -7;
    private const int CoseP256Curve = 1;

    private readonly ECDsa _privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
    private readonly byte[] _credentialId = RandomNumberGenerator.GetBytes(CredentialIdByteCount);
    private byte[] _userHandle = [];
    private uint _signCount;

    public string CredentialId => Base64Url.EncodeToString(_credentialId);

    public string Origin { get; set; } = AllowedOrigin;

    public bool VerifiesUser { get; set; } = true;

    public JsonObject CreateCredential(JsonElement creationOptions)
    {
        _userHandle = Base64Url.DecodeFromChars(creationOptions.GetProperty("user").GetProperty("id").GetString());
        var clientDataJson = ClientDataJson(CreateCeremonyType, creationOptions.GetProperty("challenge").GetString()!);
        var authenticatorData = AuthenticatorData(AttestedCredentialDataFlag, AttestedCredentialData());
        var attestationObject = new CborWriter()
            .Map(3)
            .Text("fmt").Text(NoneAttestationFormat)
            .Text("attStmt").Map(0)
            .Text("authData").Bytes(authenticatorData)
            .ToArray();

        return Credential(new JsonObject
        {
            ["clientDataJSON"] = Base64Url.EncodeToString(clientDataJson),
            ["attestationObject"] = Base64Url.EncodeToString(attestationObject),
            ["transports"] = new JsonArray(InternalTransport),
        });
    }

    public JsonObject Sign(JsonElement requestOptions)
    {
        _signCount++;
        var clientDataJson = ClientDataJson(GetCeremonyType, requestOptions.GetProperty("challenge").GetString()!);
        var authenticatorData = AuthenticatorData(0, []);
        var signedData = authenticatorData.Concat(SHA256.HashData(clientDataJson)).ToArray();
        var signature = _privateKey.SignData(signedData, HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);

        return Credential(new JsonObject
        {
            ["clientDataJSON"] = Base64Url.EncodeToString(clientDataJson),
            ["authenticatorData"] = Base64Url.EncodeToString(authenticatorData),
            ["signature"] = Base64Url.EncodeToString(signature),
            ["userHandle"] = Base64Url.EncodeToString(_userHandle),
        });
    }

    public void Dispose() => _privateKey.Dispose();

    private JsonObject Credential(JsonObject response) => new()
    {
        ["id"] = CredentialId,
        ["rawId"] = CredentialId,
        ["type"] = PublicKeyCredentialType,
        ["authenticatorAttachment"] = PlatformAttachment,
        ["response"] = response,
        ["clientExtensionResults"] = new JsonObject(),
    };

    private byte[] ClientDataJson(string ceremonyType, string challenge) =>
        Encoding.UTF8.GetBytes(new JsonObject
        {
            ["type"] = ceremonyType,
            ["challenge"] = challenge,
            ["origin"] = Origin,
            ["crossOrigin"] = false,
        }.ToJsonString());

    private byte[] AuthenticatorData(byte extraFlags, byte[] attestedCredentialData)
    {
        var flags = (byte)(UserPresentFlag | (VerifiesUser ? UserVerifiedFlag : 0) | extraFlags);
        var signCount = new byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(signCount, _signCount);

        return [.. SHA256.HashData(Encoding.UTF8.GetBytes(RelyingPartyId)), flags, .. signCount, .. attestedCredentialData];
    }

    private byte[] AttestedCredentialData()
    {
        var publicKey = _privateKey.ExportParameters(includePrivateParameters: false).Q;
        var credentialIdLength = new byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(credentialIdLength, (ushort)_credentialId.Length);
        var coseKey = new CborWriter()
            .Map(5)
            .Integer(CoseKeyTypeLabel).Integer(CoseEllipticCurveKeyType)
            .Integer(CoseAlgorithmLabel).Integer(CoseEs256Algorithm)
            .Integer(CoseCurveLabel).Integer(CoseP256Curve)
            .Integer(CoseXLabel).Bytes(publicKey.X!)
            .Integer(CoseYLabel).Bytes(publicKey.Y!)
            .ToArray();

        return [.. new byte[AuthenticatorGuidByteCount], .. credentialIdLength, .. _credentialId, .. coseKey];
    }

    private sealed class CborWriter
    {
        private const byte UnsignedIntegerMajorType = 0;
        private const byte NegativeIntegerMajorType = 1;
        private const byte ByteStringMajorType = 2;
        private const byte TextStringMajorType = 3;
        private const byte MapMajorType = 5;
        private const int InlineLengthLimit = 24;
        private const byte OneByteLength = 24;
        private const byte TwoByteLength = 25;

        private readonly List<byte> _bytes = [];

        public CborWriter Map(int entryCount) => Head(MapMajorType, entryCount);

        public CborWriter Integer(int value) =>
            value >= 0 ? Head(UnsignedIntegerMajorType, value) : Head(NegativeIntegerMajorType, -1 - value);

        public CborWriter Text(string value)
        {
            var utf8 = Encoding.UTF8.GetBytes(value);
            Head(TextStringMajorType, utf8.Length);
            _bytes.AddRange(utf8);
            return this;
        }

        public CborWriter Bytes(byte[] value)
        {
            Head(ByteStringMajorType, value.Length);
            _bytes.AddRange(value);
            return this;
        }

        public byte[] ToArray() => [.. _bytes];

        private CborWriter Head(byte majorType, int argument)
        {
            var typeBits = (byte)(majorType << 5);
            if (argument < InlineLengthLimit)
            {
                _bytes.Add((byte)(typeBits | argument));
            }
            else if (argument <= byte.MaxValue)
            {
                _bytes.Add((byte)(typeBits | OneByteLength));
                _bytes.Add((byte)argument);
            }
            else
            {
                _bytes.Add((byte)(typeBits | TwoByteLength));
                _bytes.Add((byte)(argument >> 8));
                _bytes.Add((byte)argument);
            }

            return this;
        }
    }
}
