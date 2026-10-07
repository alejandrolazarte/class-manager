using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record DeclineStudentAppInvitationCommand(string? Token) : ICommand;

public sealed record DeclinedStudentAppInvitationResponse;

public sealed class DeclineStudentAppInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<DeclineStudentAppInvitationCommand, DeclinedStudentAppInvitationResponse>
{
    public async Task<Result<DeclinedStudentAppInvitationResponse>> ExecuteAsync(DeclineStudentAppInvitationCommand command, CancellationToken cancellationToken)
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
        return new DeclinedStudentAppInvitationResponse();
    }

    private static Result<DeclinedStudentAppInvitationResponse> InvalidInvitation() =>
        Result.Validation<DeclinedStudentAppInvitationResponse>(StudentAppErrorCodes.InvalidInvitationMessage, StudentAppErrorCodes.InvalidInvitation);
}
