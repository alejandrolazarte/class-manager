using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.Domain.Clients;

public static class FamilyVisibility
{
    public static FamilyMemberView ViewOf(bool isOwnStudent, bool viewerPaysForFamily, DateOnly? birthDate, DateOnly today, int ownAccountMinimumAge)
    {
        if (isOwnStudent)
        {
            return FamilyMemberView.Everything;
        }

        if (!viewerPaysForFamily)
        {
            return FamilyMemberView.None;
        }

        if (birthDate is not { } knownBirthDate)
        {
            return FamilyMemberView.Everything;
        }

        var years = PersonAge.YearsOn(knownBirthDate, today);
        if (years < ownAccountMinimumAge)
        {
            return FamilyMemberView.Everything;
        }

        return years < PersonAge.AdultAge ? FamilyMemberView.Classes : FamilyMemberView.None;
    }

    public static bool NeedsGuardian(DateOnly? birthDate, DateOnly today, int ownAccountMinimumAge) =>
        birthDate is not { } knownBirthDate || PersonAge.YearsOn(knownBirthDate, today) < ownAccountMinimumAge;

    public static bool CanBuy(bool viewerPaysForFamily, DateOnly? birthDate, DateOnly today) =>
        viewerPaysForFamily || (birthDate is { } knownBirthDate && PersonAge.IsAdult(knownBirthDate, today));

    public static DateOnly? YoungestBirthDate(IEnumerable<DateOnly?> birthDates) =>
        birthDates.OfType<DateOnly>().Select(birthDate => (DateOnly?)birthDate).DefaultIfEmpty(null).Max();
}
