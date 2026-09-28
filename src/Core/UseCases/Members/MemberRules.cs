using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.UseCases.Members;

internal static class MemberRules
{
    private const string RoleNotAllowedMessage = "Only a brand owner can give or take the branch owner role.";
    private const string OwnMembershipMessage = "You can't change or remove your own access.";
    private const string InstructorNotFoundMessage = "The coach does not exist.";
    private const string InstructorTakenMessage = "Another team member is already linked to this coach.";
    private const string MemberNotFoundMessage = "The team member does not exist.";

    public static ResultError RoleNotAllowed() => new(MemberErrorCodes.RoleNotAllowed, RoleNotAllowedMessage, ErrorKind.Forbidden);

    public static ResultError OwnMembership() => new(MemberErrorCodes.OwnMembership, OwnMembershipMessage, ErrorKind.Forbidden);

    public static ResultError MemberNotFound() => new(MemberErrorCodes.NotFound, MemberNotFoundMessage, ErrorKind.NotFound);

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
