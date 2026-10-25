namespace ClassManager.Security.Passkeys;

public enum PasskeyRegistrationStartStatus
{
    Started,
    InvalidPassword,
    UnknownUser,
}

public sealed record PasskeyRegistrationStart(PasskeyRegistrationStartStatus Status, PasskeyCeremonyStart? Ceremony)
{
    public static PasskeyRegistrationStart InvalidPassword { get; } = new(PasskeyRegistrationStartStatus.InvalidPassword, null);

    public static PasskeyRegistrationStart UnknownUser { get; } = new(PasskeyRegistrationStartStatus.UnknownUser, null);

    public static PasskeyRegistrationStart Started(PasskeyCeremonyStart ceremony) => new(PasskeyRegistrationStartStatus.Started, ceremony);
}
