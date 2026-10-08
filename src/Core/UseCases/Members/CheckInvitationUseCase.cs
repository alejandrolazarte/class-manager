using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Members;

public sealed record CheckInvitationCommand(string? Token) : ICommand;

public sealed record CheckInvitationResponse(string Email, string BusinessName, bool HasAccount, string? FullName = null);

public sealed class CheckInvitationUseCase(
    IMemberInvitationRepository invitationRepository,
    IBusinessRepository businessRepository,
    IInstructorRepository instructorRepository,
    IIdentityService identityService,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
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
        if (invitation is null || !invitation.IsPendingAt(timeProvider.GetUtcNow()))
        {
            return InvalidInvitation();
        }

        tenantScope.Establish(invitation.TenantId);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        var hasAccount = await identityService.IsEmailRegisteredAsync(invitation.Email, cancellationToken);
        var fullName = await InvitedPersonName.FindAsync(invitation, instructorRepository, cancellationToken);
        return new CheckInvitationResponse(invitation.Email, business?.BrandDisplayName ?? string.Empty, hasAccount, fullName);
    }

    private static Result<CheckInvitationResponse> InvalidInvitation() =>
        Result.Validation<CheckInvitationResponse>(InvalidInvitationMessage, MemberErrorCodes.InvalidInvitation);
}
