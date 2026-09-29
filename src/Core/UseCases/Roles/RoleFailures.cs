using ClassManager.Core.Common;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.UseCases.Roles;

internal static class RoleFailures
{
    private const string NotFoundMessage = "The role does not exist.";
    private const string NameTakenMessage = "Another role already has this name.";
    private const string OwnRoleMessage = "You can't change the role you have.";
    private const string InUseMessage = "Team members or pending invitations use this role. Give them another role first.";
    private const string InstructorRequiredMessage = "These permissions only show a coach's own classes, and some team members with this role have no linked coach.";

    public static ResultError NotFound() => new(RoleErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError NameTaken() => new(RoleErrorCodes.NameTaken, NameTakenMessage, ErrorKind.Conflict) { FieldName = nameof(CustomRole.Name) };

    public static ResultError OwnRole() => new(RoleErrorCodes.OwnRole, OwnRoleMessage, ErrorKind.Forbidden);

    public static ResultError InUse() => new(RoleErrorCodes.InUse, InUseMessage, ErrorKind.Conflict);

    public static ResultError InstructorRequired(int memberCount) =>
        new(RoleErrorCodes.InstructorRequired, InstructorRequiredMessage, ErrorKind.Conflict)
        {
            Details = new Dictionary<string, object?> { [RoleErrorCodes.MemberCountDetail] = memberCount },
        };
}
