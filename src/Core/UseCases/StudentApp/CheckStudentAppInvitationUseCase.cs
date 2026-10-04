using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record CheckStudentAppInvitationCommand(string? Token);

public sealed record CheckStudentAppInvitationResponse;

public sealed class CheckStudentAppInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    ISecretTokenGenerator secretTokenGenerator,
    TimeProvider timeProvider)
    : IUseCase<CheckStudentAppInvitationCommand, CheckStudentAppInvitationResponse>
{
    public async Task<Result<CheckStudentAppInvitationResponse>> ExecuteAsync(CheckStudentAppInvitationCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return InvalidInvitation();
        }

        var invitation = await invitationRepository.FindForUpdateInAnyBusinessByTokenHashAsync(
            secretTokenGenerator.Hash(command.Token), cancellationToken);

        return invitation is not null && invitation.IsPendingAt(timeProvider.GetUtcNow())
            ? new CheckStudentAppInvitationResponse()
            : InvalidInvitation();
    }

    private static Result<CheckStudentAppInvitationResponse> InvalidInvitation() =>
        Result.Validation<CheckStudentAppInvitationResponse>(StudentAppErrorCodes.InvalidInvitationMessage, StudentAppErrorCodes.InvalidInvitation);
}
