namespace ClassManager.Backoffice.Core.Operators;

public static class OperatorErrorCodes
{
    public const string AccessRequestNotFound = "backoffice.access_request_not_found";
    public const string AccessRequestNotFoundMessage = "There is no access request with that id.";
    public const string LoginNotFound = "backoffice.operator_login_not_found";
    public const string LoginNotFoundMessage = "There is no enabled identity with that id.";
    public const string UnknownTeamMember = "backoffice.unknown_platform_team_member";
    public const string UnknownTeamMemberMessage = "The person must be a team member of a branch of the platform organization, with an account.";
    public const string NotAPlatformRole = "backoffice.not_a_platform_role";
    public const string NotAPlatformRoleMessage = "The role must be PlatformOperator or PlatformReader.";
    public const string CannotDisableYourself = "backoffice.cannot_disable_yourself";
    public const string CannotDisableYourselfMessage = "An operator can't disable the identity of their own account.";
}
