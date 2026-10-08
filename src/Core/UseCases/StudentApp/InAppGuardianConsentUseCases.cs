using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GiveGuardianConsentInAppCommand(Guid InvitationId) : ICommand;

public sealed record RefuseGuardianConsentInAppCommand(Guid InvitationId) : ICommand;

internal static class InAppGuardianConsentRequests
{
    private const string NotFoundMessage = "There is no pending authorization with this id for this account.";

    public static async Task<ClientInvitation?> FindPendingForClientAsync(
        IStudentAppAccess studentAppAccess,
        IClientAccountRepository clientAccountRepository,
        IClientInvitationRepository invitationRepository,
        Guid invitationId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        var account = access is null ? null : await clientAccountRepository.FindByUserAsync(access.UserId, cancellationToken);
        if (access is null || account is null || account.StudentId is not null)
        {
            return null;
        }

        var invitation = await invitationRepository.FindForUpdateAsync(invitationId, cancellationToken);
        return invitation is not null
            && invitation.ClientId == access.ClientId
            && invitation.IsPendingAt(now)
            && invitation.AwaitsGuardianConsent
                ? invitation
                : null;
    }

    public static Result<TResponse> NotFound<TResponse>() =>
        Result.NotFound<TResponse>(NotFoundMessage, StudentAppErrorCodes.InvalidInvitation);
}

public sealed class GiveGuardianConsentInAppUseCase(
    IStudentAppAccess studentAppAccess,
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    IBusinessRepository businessRepository,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<GiveGuardianConsentInAppCommand, GivenGuardianConsentResponse>
{
    public async Task<Result<GivenGuardianConsentResponse>> ExecuteAsync(GiveGuardianConsentInAppCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var invitation = await InAppGuardianConsentRequests.FindPendingForClientAsync(
            studentAppAccess, clientAccountRepository, invitationRepository, command.InvitationId, now, cancellationToken);
        var business = invitation is null ? null : await businessRepository.GetCurrentAsync(cancellationToken);
        if (invitation is null || business is null)
        {
            return InAppGuardianConsentRequests.NotFound<GivenGuardianConsentResponse>();
        }

        return await GuardianConsentActions.GiveAsync(
            invitation, business, secretTokenGenerator, emailSender, webAppLinks, unitOfWork, now, cancellationToken);
    }
}

public sealed class RefuseGuardianConsentInAppUseCase(
    IStudentAppAccess studentAppAccess,
    IClientAccountRepository clientAccountRepository,
    IClientInvitationRepository invitationRepository,
    ITeamNotificationService teamNotificationService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<RefuseGuardianConsentInAppCommand, RefusedGuardianConsentResponse>
{
    public async Task<Result<RefusedGuardianConsentResponse>> ExecuteAsync(RefuseGuardianConsentInAppCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var invitation = await InAppGuardianConsentRequests.FindPendingForClientAsync(
            studentAppAccess, clientAccountRepository, invitationRepository, command.InvitationId, now, cancellationToken);
        if (invitation is null)
        {
            return InAppGuardianConsentRequests.NotFound<RefusedGuardianConsentResponse>();
        }

        return await GuardianConsentActions.RefuseAsync(invitation, teamNotificationService, unitOfWork, now, cancellationToken);
    }
}
