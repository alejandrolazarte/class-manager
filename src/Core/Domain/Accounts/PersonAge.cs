using ClassManager.Core.Domain.Countries;

namespace ClassManager.Core.Domain.Accounts;

public static class PersonAge
{
    public const int AdultAge = 18;
    public static readonly DateOnly EarliestBirthDate = new(1900, 1, 1);

    public static bool IsValidBirthDate(DateOnly birthDate, DateOnly today) =>
        birthDate >= EarliestBirthDate && birthDate <= today;

    public static int YearsOn(DateOnly birthDate, DateOnly today)
    {
        var years = today.Year - birthDate.Year;
        return birthDate.AddYears(years) > today ? years - 1 : years;
    }

    public static bool CanHaveOwnAccount(DateOnly birthDate, DateOnly today, Country country) =>
        YearsOn(birthDate, today) >= country.OwnAccountMinimumAge;

    public static bool IsAdult(DateOnly birthDate, DateOnly today) =>
        YearsOn(birthDate, today) >= AdultAge;
}
