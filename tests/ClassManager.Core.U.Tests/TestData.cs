using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests;

internal static class TestData
{
    public const string DefaultCountryCallingCode = "54";
    public const string BuenosAiresTimeZoneId = "America/Argentina/Buenos_Aires";
    public const string CurrencyCode = "ARS";
    public const string ClientFullName = "Ana Pérez";
    public const string ClientPhoneNumber = "11 2233-4455";
    public const string NormalizedClientPhoneNumber = "+541122334455";
    public const string OwnerFullName = "Laura Gómez";
    public const string OwnerEmail = "laura@example.com";
    public const string OwnerPassword = "a long passphrase";
    public const string BusinessName = "Panadería Laura";
    public const string StudentFullName = "Tomás Pérez";
    public const string InstructorFullName = "Laura Gómez";

    public static readonly DateTimeOffset Now = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);
    public static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);

    public static Business Business() =>
        ClassManager.Core.Domain.Businesses.Business.Create("Demo business", "demo-business", BuenosAiresTimeZoneId, CurrencyCode, DefaultCountryCallingCode, Now).Value!;

    public static PhoneNumber PhoneNumber() =>
        ClassManager.Core.Domain.Clients.PhoneNumber.Create(ClientPhoneNumber, DefaultCountryCallingCode).Value!;

    public static Client Client() =>
        ClassManager.Core.Domain.Clients.Client.Create(ClientFullName, PhoneNumber(), null, null, Now).Value!;

    public static IssuedTokens IssuedTokens() =>
        new("access-token", Now.AddMinutes(15), "refresh-token", Now.AddDays(30));
}
