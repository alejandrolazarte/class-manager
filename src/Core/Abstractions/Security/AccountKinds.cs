namespace ClassManager.Core.Abstractions.Security;

public static class AccountKinds
{
    public const string Team = "team";
    public const string Family = "family";
    public const string FamilyRoleName = "Family";

    public static bool IsTeam(string? kind) => kind is null or Team;

    public static bool IsFamily(string? kind) => kind == Family;
}
