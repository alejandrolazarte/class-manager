using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Infrastructure.Security;

internal sealed class SecretTokenGenerator : ISecretTokenGenerator
{
    private const int TokenByteCount = 32;

    public SecretToken Create()
    {
        var value = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenByteCount));
        return new SecretToken(value, Hash(value));
    }

    public string Hash(string tokenValue) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenValue)));
}
