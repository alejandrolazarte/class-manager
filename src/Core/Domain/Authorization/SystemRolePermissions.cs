using System.Collections.Frozen;

using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Domain.Authorization;

public static class SystemRolePermissions
{
    private static readonly FrozenSet<string> EveryPermission = Permissions.All.ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> BranchOwnerPermissions =
        Permissions.All.Except(Permissions.BrandOnly).ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> CoachPermissions = new[]
    {
        Permissions.Business.View,
        Permissions.Instructors.View,
        Permissions.ClassGroups.ViewOwn,
        Permissions.Enrollments.View,
        Permissions.Sessions.ViewOwn,
        Permissions.Attendance.RecordOwn,
        Permissions.PrivateLessons.ViewOwn,
        Permissions.PrivateLessons.ManageOwn,
        Permissions.Students.ViewOwn,
        Permissions.Students.Manage,
    }.ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> ViewerPermissions = new[]
    {
        Permissions.Business.View,
        Permissions.Members.View,
        Permissions.Instructors.View,
        Permissions.ClassGroups.ViewAll,
        Permissions.Enrollments.View,
        Permissions.Sessions.ViewAll,
        Permissions.PrivateLessons.ViewAll,
        Permissions.Students.ViewAll,
        Permissions.Payments.ViewAll,
        Permissions.ClassPacks.View,
    }.ToFrozenSet(StringComparer.Ordinal);

    public static IReadOnlySet<string> BrandOwner => EveryPermission;

    public static IReadOnlySet<string> Of(BusinessRole role) =>
        role switch
        {
            BusinessRole.BranchOwner => BranchOwnerPermissions,
            BusinessRole.Coach => CoachPermissions,
            BusinessRole.Viewer => ViewerPermissions,
            _ => FrozenSet<string>.Empty,
        };

    public static bool NeedsInstructor(BusinessRole role)
    {
        var permissions = Of(role);
        return Permissions.EveryInstructorPermissionByOwnPermission.Any(pair =>
            permissions.Contains(pair.Key) && !permissions.Contains(pair.Value));
    }
}
