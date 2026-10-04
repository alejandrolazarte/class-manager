using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Roles;

public sealed record DeleteRoleCommand(Guid RoleId) : ICommand;

public sealed class DeleteRoleUseCase(
    ICustomRoleRepository customRoleRepository,
    IBusinessMemberRepository businessMemberRepository,
    IMemberInvitationRepository invitationRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<DeleteRoleCommand, DeletedRoleResponse>
{
    public async Task<Result<DeletedRoleResponse>> ExecuteAsync(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await customRoleRepository.GetForUpdateAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            return RoleFailures.NotFound();
        }

        var members = await businessMemberRepository.ListByCustomRoleAsync(role.Id, cancellationToken);
        var invitations = await invitationRepository.ListPendingByCustomRoleAsync(role.Id, timeProvider.GetUtcNow(), cancellationToken);
        if (members.Count > 0 || invitations.Count > 0)
        {
            return RoleFailures.InUse();
        }

        customRoleRepository.Remove(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeletedRoleResponse(role.Id);
    }
}
