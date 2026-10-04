using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record RevokeInvitationCommand(Guid InvitationId) : ICommand;

public sealed record RevokedInvitationResponse(Guid Id);

public sealed class RevokeInvitationUseCase(
    IMemberInvitationRepository invitationRepository,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<RevokeInvitationCommand, RevokedInvitationResponse>
{
    private const string NotFoundMessage = "The invitation does not exist or was already used.";

    public async Task<Result<RevokedInvitationResponse>> ExecuteAsync(RevokeInvitationCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var invitation = await invitationRepository.GetForUpdateAsync(command.InvitationId, cancellationToken);
        if (invitation is null || !invitation.IsPendingAt(now))
        {
            return Result.NotFound<RevokedInvitationResponse>(NotFoundMessage, MemberErrorCodes.InvitationNotFound);
        }

        if (!await MemberRules.CanManageRoleAsync(currentMember, invitation.Role, cancellationToken))
        {
            return MemberRules.RoleNotAllowed();
        }

        invitation.Revoke(now);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RevokedInvitationResponse(invitation.Id);
    }
}
