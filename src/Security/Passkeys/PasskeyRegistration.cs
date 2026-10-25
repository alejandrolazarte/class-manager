namespace ClassManager.Security.Passkeys;

public enum PasskeyRegistrationStatus
{
    Registered,
    InvalidCeremony,
    InvalidCredential,
}

public sealed record PasskeyRegistration(PasskeyRegistrationStatus Status, UserPasskeySummary? Passkey, string? Email)
{
    public static PasskeyRegistration InvalidCeremony { get; } = new(PasskeyRegistrationStatus.InvalidCeremony, null, null);

    public static PasskeyRegistration InvalidCredential { get; } = new(PasskeyRegistrationStatus.InvalidCredential, null, null);

    public static PasskeyRegistration Registered(UserPasskeySummary passkey, string email) =>
        new(PasskeyRegistrationStatus.Registered, passkey, email);
}
