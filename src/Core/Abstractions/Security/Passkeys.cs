namespace ClassManager.Core.Abstractions.Security;

public sealed record StartedPasskeyCeremony(string CeremonyId, string OptionsJson);

public sealed record AccountPasskey(string Id, string Name, DateTimeOffset CreatedAt);

public sealed record AddedPasskey(AccountPasskey Passkey, string Email);
