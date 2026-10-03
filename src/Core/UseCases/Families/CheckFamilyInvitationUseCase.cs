using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Families;

public sealed record CheckFamilyInvitationCommand(string? Token);

public sealed record CheckFamilyInvitationResponse;

public sealed class CheckFamilyInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    ISecretTokenGenerator secretTokenGenerator,
    TimeProvider timeProvider)
    : IUseCase<CheckFamilyInvitationCommand, CheckFamilyInvitationResponse>
{
    public async Task<Result<CheckFamilyInvitationResponse>> ExecuteAsync(CheckFamilyInvitationCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return InvalidInvitation();
        }

        var invitation = await invitationRepository.FindForUpdateInAnyBusinessByTokenHashAsync(
            secretTokenGenerator.Hash(command.Token), cancellationToken);

        return invitation is not null && invitation.IsPendingAt(timeProvider.GetUtcNow())
            ? new CheckFamilyInvitationResponse()
            : InvalidInvitation();
    }

    private static Result<CheckFamilyInvitationResponse> InvalidInvitation() =>
        Result.Validation<CheckFamilyInvitationResponse>(FamilyErrorCodes.InvalidInvitationMessage, FamilyErrorCodes.InvalidInvitation);
}
