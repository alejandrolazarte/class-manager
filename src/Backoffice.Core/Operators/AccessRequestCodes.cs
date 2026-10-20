using System.Security.Cryptography;

namespace ClassManager.Backoffice.Core.Operators;

public static class AccessRequestCodes
{
    public const char Separator = '-';

    private const string UnambiguousCharacters = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
    private const int FirstPartLength = 2;
    private const int SecondPartLength = 3;

    public static string Create() =>
        new string(RandomNumberGenerator.GetItems<char>(UnambiguousCharacters, FirstPartLength))
        + Separator
        + new string(RandomNumberGenerator.GetItems<char>(UnambiguousCharacters, SecondPartLength));
}
