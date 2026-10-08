namespace ClassManager.Core.Domain.Accounts;

public static class PersonAge
{
    public const int AdultAge = 18;
    public const int DefaultOwnAccountMinimumAge = 16;

    public static readonly DateOnly EarliestBirthDate = new(1900, 1, 1);

    private const string ArgentinaCallingCode = "54";
    private const string SpainCallingCode = "34";
    private const int ArgentinaOwnAccountMinimumAge = 13;
    private const int SpainOwnAccountMinimumAge = 14;

    private static readonly IReadOnlyDictionary<string, int> OwnAccountMinimumAgeByCallingCode = new Dictionary<string, int>
    {
        [ArgentinaCallingCode] = ArgentinaOwnAccountMinimumAge,
        [SpainCallingCode] = SpainOwnAccountMinimumAge,
    };

    public static bool IsValidBirthDate(DateOnly birthDate, DateOnly today) =>
        birthDate >= EarliestBirthDate && birthDate <= today;

    public static int YearsOn(DateOnly birthDate, DateOnly today)
    {
        var years = today.Year - birthDate.Year;
        return birthDate.AddYears(years) > today ? years - 1 : years;
    }

    public static int OwnAccountMinimumAge(string countryCallingCode) =>
        OwnAccountMinimumAgeByCallingCode.GetValueOrDefault(countryCallingCode, DefaultOwnAccountMinimumAge);

    public static bool CanHaveOwnAccount(DateOnly birthDate, DateOnly today, string countryCallingCode) =>
        YearsOn(birthDate, today) >= OwnAccountMinimumAge(countryCallingCode);

    public static bool IsAdult(DateOnly birthDate, DateOnly today) =>
        YearsOn(birthDate, today) >= AdultAge;
}
