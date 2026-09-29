using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.UseCases.Members;

internal static class MemberRules
{
    private const string RoleNotAllowedMessage = "Only a brand owner can give or take the branch owner role.";
    private const string OwnMembershipMessage = "You can't change or remove your own access.";
    private const string InstructorNotFoundMessage = "The coach does not exist.";
    private const string InstructorTakenMessage = "Another team member is already linked to this coach.";
    private const string MemberNotFoundMessage = "The team member does not exist.";
    private const string RoleRequiredMessage = "Choose a role.";
    private const string CustomRoleNotFoundMessage = "The role does not exist.";
    private const string ExceedsOwnMessage = "You can't give permissions you don't have.";

    public static ResultError RoleNotAllowed() => new(MemberErrorCodes.RoleNotAllowed, RoleNotAllowedMessage, ErrorKind.Forbidden);

    public static ResultError OwnMembership() => new(MemberErrorCodes.OwnMembership, OwnMembershipMessage, ErrorKind.Forbidden);

    public static ResultError MemberNotFound() => new(MemberErrorCodes.NotFound, MemberNotFoundMessage, ErrorKind.NotFound);

    public static ResultError ExceedsOwn() => new(RoleErrorCodes.ExceedsOwn, ExceedsOwnMessage, ErrorKind.Forbidden);

    public static async Task<Result<MemberRole>> ResolveRoleAsync(
        ICustomRoleRepository customRoleRepository,
        BusinessRole? role,
        Guid? customRoleId,
        CancellationToken cancellationToken)
    {
        if (customRoleId is { } id && role is null or BusinessRole.Custom)
        {
            var customRole = await customRoleRepository.GetByIdAsync(id, cancellationToken);
            return customRole is null
                ? Result.NotFound<MemberRole>(CustomRoleNotFoundMessage, RoleErrorCodes.NotFound)
                : MemberRole.Custom(customRole);
        }

        return role is { } systemRole && systemRole != BusinessRole.Custom && customRoleId is null
            ? MemberRole.System(systemRole)
            : Result.Validation<MemberRole>(RoleRequiredMessage, fieldName: nameof(BusinessMember.Role));
    }

    public static async Task<ResultError?> CheckCanGiveAsync(ICurrentMember currentMember, MemberRole role, CancellationToken cancellationToken)
    {
        if (!await CanManageRoleAsync(currentMember, role.Role, cancellationToken))
        {
            return RoleNotAllowed();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        return access is not null && role.Permissions.IsSubsetOf(access.Permissions) ? null : ExceedsOwn();
    }

    public static async Task<bool> CanManageRoleAsync(ICurrentMember currentMember, BusinessRole role, CancellationToken cancellationToken)
    {
        if (role != BusinessRole.BranchOwner)
        {
            return true;
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        return access?.HasPermission(Permissions.Members.ManageBranchOwners) == true;
    }

    public static async Task<ResultError?> ValidateInstructorAsync(
        IInstructorRepository instructorRepository,
        IBusinessMemberRepository businessMemberRepository,
        Guid? instructorId,
        Guid? exceptMemberId,
        CancellationToken cancellationToken)
    {
        if (instructorId is not { } id)
        {
            return null;
        }

        if (await instructorRepository.GetByIdAsync(id, cancellationToken) is null)
        {
            return new ResultError(InstructorErrorCodes.NotFound, InstructorNotFoundMessage, ErrorKind.NotFound);
        }

        return await businessMemberRepository.IsInstructorLinkedAsync(id, exceptMemberId, cancellationToken)
            ? new ResultError(MemberErrorCodes.InstructorTaken, InstructorTakenMessage, ErrorKind.Conflict)
            : null;
    }
}
