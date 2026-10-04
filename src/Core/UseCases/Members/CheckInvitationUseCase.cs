using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record CheckInvitationCommand(string? Token) : ICommand;

public sealed record CheckInvitationResponse;

public sealed class CheckInvitationUseCase(
    IMemberInvitationRepository invitationRepository,
    ISecretTokenGenerator secretTokenGenerator,
    TimeProvider timeProvider)
    : IUseCase<CheckInvitationCommand, CheckInvitationResponse>
{
    private const string InvalidInvitationMessage = "The invitation link is invalid, expired or already used. Ask for a new one.";

    public async Task<Result<CheckInvitationResponse>> ExecuteAsync(CheckInvitationCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return InvalidInvitation();
        }

        var invitation = await invitationRepository.FindForUpdateInAnyBusinessByTokenHashAsync(
            secretTokenGenerator.Hash(command.Token), cancellationToken);

        return invitation is not null && invitation.IsPendingAt(timeProvider.GetUtcNow())
            ? new CheckInvitationResponse()
            : InvalidInvitation();
    }

    private static Result<CheckInvitationResponse> InvalidInvitation() =>
        Result.Validation<CheckInvitationResponse>(InvalidInvitationMessage, MemberErrorCodes.InvalidInvitation);
}
