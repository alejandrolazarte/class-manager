namespace ClassManager.Core.Abstractions.Security;

public static class AccountKinds
{
    public const string Team = "team";
    public const string Student = "student";
    public const string StudentRoleName = "Student";

    public static bool IsTeam(string? kind) => kind is null or Team;

    public static bool IsStudent(string? kind) => kind == Student;
}
