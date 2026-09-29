namespace ClassManager.Api.Authentication;

internal static class AuthorizationPolicies
{
    public const string PermissionPrefix = "permission:";
    public const string Member = PermissionPrefix;
    public const string Family = "family";

    private const char PermissionSeparator = ',';

    public static string AnyOf(IEnumerable<string> permissions) => PermissionPrefix + string.Join(PermissionSeparator, permissions);

    public static IReadOnlyList<string> PermissionsOf(string policyName) =>
        policyName[PermissionPrefix.Length..].Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries);
}
