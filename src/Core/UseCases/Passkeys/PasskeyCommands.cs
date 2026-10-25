using System.Text.Json;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Passkeys;

internal static class PasskeyCommands
{
    public const string CeremonyRequiredMessage = "The passkey request is missing.";
    public const string CredentialRequiredMessage = "The passkey answer is missing.";

    public static bool HasCredential(JsonElement? credential) => credential is { ValueKind: JsonValueKind.Object };

    public static Result<T> MissingCeremony<T>(string fieldName) => Result.Validation<T>(CeremonyRequiredMessage, fieldName: fieldName);

    public static Result<T> MissingCredential<T>(string fieldName) => Result.Validation<T>(CredentialRequiredMessage, fieldName: fieldName);
}
