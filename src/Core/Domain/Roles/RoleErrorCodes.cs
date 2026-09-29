namespace ClassManager.Core.Domain.Roles;

public static class RoleErrorCodes
{
    public const string NotFound = "role.not_found";
    public const string NameTaken = "role.name_taken";
    public const string NameReserved = "role.name_reserved";
    public const string PermissionsRequired = "role.permissions_required";
    public const string PermissionNotAssignable = "role.permission_not_assignable";
    public const string InUse = "role.in_use";
    public const string InstructorRequired = "role.instructor_required";
    public const string OwnRole = "role.own_role";
    public const string ExceedsOwn = "role.exceeds_own";
    public const string PermissionDetail = "permission";
    public const string MemberCountDetail = "memberCount";
}
