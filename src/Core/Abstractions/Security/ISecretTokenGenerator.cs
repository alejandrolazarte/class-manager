namespace ClassManager.Core.Abstractions.Security;

public interface ISecretTokenGenerator
{
    SecretToken Create();

    string Hash(string tokenValue);
}
