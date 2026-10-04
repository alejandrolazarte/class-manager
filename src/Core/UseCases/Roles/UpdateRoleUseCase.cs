using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Roles;

public sealed record UpdateRoleRequest(string? Name, IReadOnlyList<string>? Permissions);

public sealed record UpdateRoleCommand(Guid RoleId, string? Name, IReadOnlyList<string>? Permissions) : ICommand;

public sealed class UpdateRoleUseCase(
    ICustomRoleRepository customRoleRepository,
    IBusinessMemberRepository businessMemberRepository,
    IMemberInvitationRepository invitationRepository,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<UpdateRoleCommand, RoleResponse>
{
    public async Task<Result<RoleResponse>> ExecuteAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await customRoleRepository.GetForUpdateAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            return RoleFailures.NotFound();
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access?.CustomRoleId == role.Id)
        {
            return RoleFailures.OwnRole();
        }

        var update = role.Update(command.Name, command.Permissions);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        if (await RoleRules.CheckCanGrantAsync(currentMember, role, cancellationToken) is { } grantError)
        {
            return grantError;
        }

        var members = await businessMemberRepository.ListByCustomRoleAsync(role.Id, cancellationToken);
        if (MemberRole.Custom(role).NeedsInstructor)
        {
            var invitations = await invitationRepository.ListPendingByCustomRoleAsync(role.Id, timeProvider.GetUtcNow(), cancellationToken);
            var withoutInstructorCount = members.Count(member => member.InstructorId is null)
                + invitations.Count(invitation => invitation.InstructorId is null);
            if (withoutInstructorCount > 0)
            {
                return RoleFailures.InstructorRequired(withoutInstructorCount);
            }
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return RoleFailures.NameTaken();
        }

        return RoleResponse.From(role, members.Count);
    }
}
