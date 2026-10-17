using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Countries;

namespace ClassManager.Core.U.Tests;

internal static class TestData
{
    public const string DefaultCountryCallingCode = "54";
    public const string BuenosAiresTimeZoneId = "America/Argentina/Buenos_Aires";
    public const string CurrencyCode = "ARS";
    public const string ArgentinaCountryCode = "AR";
    public const string SpainCountryCode = "ES";
    public const string SpainCallingCode = "34";
    public const string MadridTimeZoneId = "Europe/Madrid";
    public const string EuroCurrencyCode = "EUR";
    public const int ArgentinaOwnAccountMinimumAge = 13;
    public const int SpainOwnAccountMinimumAge = 14;
    public const string ClientFullName = "Ana Pérez";
    public const string ClientPhoneNumber = "11 2233-4455";
    public const string NormalizedClientPhoneNumber = "+541122334455";
    public const string OwnerFullName = "Laura Gómez";
    public const string OwnerEmail = "laura@example.com";
    public const string OwnerPassword = "a long passphrase";
    public static readonly DateOnly OwnerBirthDate = new(1990, 4, 12);
    public const string BusinessName = "Panadería Laura";
    public const string StudentFullName = "Tomás Pérez";
    public const string InstructorFullName = "Laura Gómez";

    public static readonly DateTimeOffset Now = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);
    public static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);

    public static Country Argentina() =>
        Country.Create(ArgentinaCountryCode, 1, DefaultCountryCallingCode, CurrencyCode, ArgentinaOwnAccountMinimumAge, BuenosAiresTimeZoneId);

    public static Country Spain() =>
        Country.Create(SpainCountryCode, 2, SpainCallingCode, EuroCurrencyCode, SpainOwnAccountMinimumAge, MadridTimeZoneId);

    public static Business Business() =>
        ClassManager.Core.Domain.Businesses.Business.Create(Guid.CreateVersion7(), "Demo business", "demo-business", Argentina(), BuenosAiresTimeZoneId, CurrencyCode, Now).Value!;

    public static PhoneNumber PhoneNumber() =>
        ClassManager.Core.Domain.Clients.PhoneNumber.Create(ClientPhoneNumber, DefaultCountryCallingCode).Value!;

    public static User UserOfAccount(Guid accountId) => User.CreateForAccount(accountId, ClientFullName, null, Now);

    public static Client Client() =>
        ClassManager.Core.Domain.Clients.Client.Create(ClientFullName, PhoneNumber(), null, null, Now).Value!;

    public static PendingSignUp PendingSignUp(string tokenHash, DateTimeOffset? createdAt = null) =>
        ClassManager.Core.Domain.Accounts.PendingSignUp.Start(
            OwnerProfile.Create(OwnerFullName, OwnerEmail, OwnerBirthDate, Today).Value!,
            new PendingBusiness(BusinessName, ArgentinaCountryCode, BuenosAiresTimeZoneId, CurrencyCode),
            tokenHash,
            createdAt ?? Now,
            LegalTerms.CurrentVersion);

    public static IssuedTokens IssuedTokens() =>
        new("access-token", Now.AddMinutes(15), "refresh-token", Now.AddDays(30));
}
