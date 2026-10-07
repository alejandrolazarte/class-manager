using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Members;

public sealed record DeclineInvitationCommand(string? Token) : ICommand;

public sealed record DeclinedInvitationResponse;

public sealed class DeclineInvitationUseCase(
    IMemberInvitationRepository invitationRepository,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<DeclineInvitationCommand, DeclinedInvitationResponse>
{
    private const string InvalidInvitationMessage = "The invitation link is invalid, expired or already used. Ask for a new one.";

    public async Task<Result<DeclinedInvitationResponse>> ExecuteAsync(DeclineInvitationCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return InvalidInvitation();
        }

        var now = timeProvider.GetUtcNow();
        var invitation = await invitationRepository.FindForUpdateInAnyBusinessByTokenHashAsync(
            secretTokenGenerator.Hash(command.Token), cancellationToken);
        if (invitation is null || !invitation.IsPendingAt(now))
        {
            return InvalidInvitation();
        }

        tenantScope.Establish(invitation.TenantId);
        invitation.Decline(now);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new DeclinedInvitationResponse();
    }

    private static Result<DeclinedInvitationResponse> InvalidInvitation() =>
        Result.Validation<DeclinedInvitationResponse>(InvalidInvitationMessage, MemberErrorCodes.InvalidInvitation);
}
