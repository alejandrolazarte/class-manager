using System.Text.Json;
using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record PasskeyCeremonyResponse(string CeremonyId, JsonElement Options)
{
    public static PasskeyCeremonyResponse From(StartedPasskeyCeremony ceremony)
    {
        using var options = JsonDocument.Parse(ceremony.OptionsJson);
        return new PasskeyCeremonyResponse(ceremony.CeremonyId, options.RootElement.Clone());
    }
}

public sealed record PasskeyResponse(string Id, string Name, DateTimeOffset CreatedAt)
{
    public static PasskeyResponse From(AccountPasskey passkey) => new(passkey.Id, passkey.Name, passkey.CreatedAt);
}
