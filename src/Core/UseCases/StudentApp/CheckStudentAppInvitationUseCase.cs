using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record CheckStudentAppInvitationCommand(string? Token) : ICommand;

public sealed record CheckStudentAppInvitationResponse(string Email, string BusinessName, bool HasAccount, string? FullName = null, DateOnly? BirthDate = null);

public sealed class CheckStudentAppInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IIdentityService identityService,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
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
        if (invitation is null || !invitation.IsPendingAt(timeProvider.GetUtcNow()))
        {
            return InvalidInvitation();
        }

        tenantScope.Establish(invitation.TenantId);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        var hasAccount = await identityService.IsEmailRegisteredAsync(invitation.Email, cancellationToken);
        var invitedStudent = await InvitedStudent.FindAsync(invitation, clientRepository, studentRepository, cancellationToken);
        return new CheckStudentAppInvitationResponse(
            invitation.Email,
            business?.BrandDisplayName ?? string.Empty,
            hasAccount,
            invitedStudent.FullName,
            invitedStudent.BirthDate);
    }

    private static Result<CheckStudentAppInvitationResponse> InvalidInvitation() =>
        Result.Validation<CheckStudentAppInvitationResponse>(StudentAppErrorCodes.InvalidInvitationMessage, StudentAppErrorCodes.InvalidInvitation);
}
