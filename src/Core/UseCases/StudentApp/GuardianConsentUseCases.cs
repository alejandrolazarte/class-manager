using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Clients;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record CheckGuardianConsentCommand(string? Token) : ICommand;

public sealed record GuardianConsentResponse(
    string BusinessName,
    string StudentFullName,
    string StudentEmail,
    string GuardianEmail,
    int MinimumAge);

public sealed record GiveGuardianConsentCommand(string? Token) : ICommand;

public sealed record GivenGuardianConsentResponse(string StudentEmail);

public sealed record RefuseGuardianConsentCommand(string? Token) : ICommand;

public sealed record RefusedGuardianConsentResponse;

internal static class GuardianConsentRequests
{
    public static async Task<ClientInvitation?> FindPendingAsync(
        IClientInvitationRepository invitationRepository,
        ISecretTokenGenerator secretTokenGenerator,
        ITenantScope tenantScope,
        string? token,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var invitation = await invitationRepository.FindForUpdateInAnyBusinessByGuardianTokenHashAsync(
            secretTokenGenerator.Hash(token), cancellationToken);
        if (invitation is null || !invitation.IsPendingAt(now) || !invitation.AwaitsGuardianConsent)
        {
            return null;
        }

        tenantScope.Establish(invitation.TenantId);
        return invitation;
    }

    public static Result<TResponse> InvalidRequest<TResponse>() =>
        Result.Validation<TResponse>(StudentAppErrorCodes.InvalidInvitationMessage, StudentAppErrorCodes.InvalidInvitation);
}

public sealed class CheckGuardianConsentUseCase(
    IClientInvitationRepository invitationRepository,
    IBusinessRepository businessRepository,
    IStudentRepository studentRepository,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
    TimeProvider timeProvider)
    : IUseCase<CheckGuardianConsentCommand, GuardianConsentResponse>
{
    public async Task<Result<GuardianConsentResponse>> ExecuteAsync(CheckGuardianConsentCommand command, CancellationToken cancellationToken)
    {
        var invitation = await GuardianConsentRequests.FindPendingAsync(
            invitationRepository, secretTokenGenerator, tenantScope, command.Token, timeProvider.GetUtcNow(), cancellationToken);
        var business = invitation is null ? null : await businessRepository.GetCurrentAsync(cancellationToken);
        var student = invitation?.StudentId is { } studentId ? await studentRepository.GetSummaryByIdAsync(studentId, cancellationToken) : null;
        if (invitation is null || business is null || student is null)
        {
            return GuardianConsentRequests.InvalidRequest<GuardianConsentResponse>();
        }

        return new GuardianConsentResponse(
            business.BrandDisplayName,
            student.FullName,
            invitation.Email,
            invitation.GuardianEmail ?? string.Empty,
            PersonAge.OwnAccountMinimumAge(business.DefaultCountryCallingCode));
    }
}

public sealed class GiveGuardianConsentUseCase(
    IClientInvitationRepository invitationRepository,
    IBusinessRepository businessRepository,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<GiveGuardianConsentCommand, GivenGuardianConsentResponse>
{
    public async Task<Result<GivenGuardianConsentResponse>> ExecuteAsync(GiveGuardianConsentCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var invitation = await GuardianConsentRequests.FindPendingAsync(
            invitationRepository, secretTokenGenerator, tenantScope, command.Token, now, cancellationToken);
        var business = invitation is null ? null : await businessRepository.GetCurrentAsync(cancellationToken);
        if (invitation is null || business is null)
        {
            return GuardianConsentRequests.InvalidRequest<GivenGuardianConsentResponse>();
        }

        var token = secretTokenGenerator.Create();
        invitation.ConsentByGuardian(token.Hash, now);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var content = InviteStudentAppUseCase.EmailContentFor(business.BrandDisplayName, webAppLinks.AcceptStudentAppInvitation(token.Value));
        await emailSender.SendAsync(
            new EmailMessage(invitation.Email, InviteStudentAppUseCase.EmailSubject, content, business.Id),
            cancellationToken);

        return new GivenGuardianConsentResponse(invitation.Email);
    }
}

public sealed class RefuseGuardianConsentUseCase(
    IClientInvitationRepository invitationRepository,
    ISecretTokenGenerator secretTokenGenerator,
    ITeamNotificationService teamNotificationService,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<RefuseGuardianConsentCommand, RefusedGuardianConsentResponse>
{
    public async Task<Result<RefusedGuardianConsentResponse>> ExecuteAsync(RefuseGuardianConsentCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var invitation = await GuardianConsentRequests.FindPendingAsync(
            invitationRepository, secretTokenGenerator, tenantScope, command.Token, now, cancellationToken);
        if (invitation is null)
        {
            return GuardianConsentRequests.InvalidRequest<RefusedGuardianConsentResponse>();
        }

        invitation.Decline(now);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await teamNotificationService.GuardianConsentRefusedAsync(invitation, cancellationToken);
        return new RefusedGuardianConsentResponse();
    }
}
