using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record InviteStudentAppRequest(string? Email)
{
    public InviteStudentAppCommand ToCommand(Guid clientId) => new(clientId, Email);
}

public sealed record InviteStudentAppCommand(Guid ClientId, string? Email) : ICommand;

public sealed class InviteStudentAppUseCase(
    IClientRepository clientRepository,
    IBusinessRepository businessRepository,
    IClientInvitationRepository invitationRepository,
    IClientAccountRepository clientAccountRepository,
    IAccessScopes accessScopes,
    ICurrentMember currentMember,
    ISecretTokenGenerator secretTokenGenerator,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<InviteStudentAppCommand, StudentAppInvitationResponse>
{
    public const string EmailSubject = "Te invitaron a la app";

    private const string ClientNotFoundMessage = "The client does not exist.";
    private const string EmailRequiredMessage = "Write the email of the person who will use the app.";
    private const string AlreadyUsesTheAppMessage = "The client already uses the app.";

    public static EmailContent EmailContentFor(string businessName, string acceptInvitationLink) =>
        new(
            "Invitación a la app",
            EmailSubject,
            $"{businessName} te invita a su app para ver las clases, tus saldos y hacer pedidos. Tocá el botón para crear tu cuenta.",
            "Si no esperabas esta invitación, podés ignorar este mail.")
        {
            Action = new EmailAction("Crear mi cuenta", acceptInvitationLink, ShowsLinkFallback: true),
            Note = new EmailNote("El link sirve una sola vez y vence en 7 días."),
        };

    public async Task<Result<StudentAppInvitationResponse>> ExecuteAsync(InviteStudentAppCommand command, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetForUpdateAsync(command.ClientId, cancellationToken);
        if (client is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [client.Id], cancellationToken))
        {
            return Result.NotFound<StudentAppInvitationResponse>(ClientNotFoundMessage, ClientErrorCodes.NotFound);
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (access is null || business is null)
        {
            return Result.Unauthorized<StudentAppInvitationResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        if (await clientAccountRepository.HasAccountAsync(client.Id, cancellationToken))
        {
            return Result.Conflict<StudentAppInvitationResponse>(AlreadyUsesTheAppMessage, StudentAppErrorCodes.AlreadyUsesTheApp);
        }

        if (!string.IsNullOrWhiteSpace(command.Email) && !client.HasEmail(command.Email))
        {
            var emailChange = client.ChangeEmail(command.Email);
            if (emailChange.IsFailure)
            {
                return emailChange.Error! with { FieldName = nameof(InviteStudentAppRequest.Email) };
            }
        }

        if (client.Email is null)
        {
            return Result.Validation<StudentAppInvitationResponse>(EmailRequiredMessage, StudentAppErrorCodes.EmailRequired, nameof(InviteStudentAppRequest.Email));
        }

        var now = timeProvider.GetUtcNow();
        var token = secretTokenGenerator.Create();
        var invitation = ClientInvitation.Create(client.Id, client.Email, token.Hash, access.UserId, now);
        if (invitation.IsFailure)
        {
            return invitation.Error!;
        }

        foreach (var previousInvitation in await invitationRepository.ListPendingForUpdateByClientAsync(client.Id, now, cancellationToken))
        {
            previousInvitation.Revoke(now);
        }

        invitationRepository.Add(invitation.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var content = EmailContentFor(business.BrandDisplayName, webAppLinks.AcceptStudentAppInvitation(token.Value));
        await emailSender.SendAsync(new EmailMessage(invitation.Value!.Email, EmailSubject, content, business.Id), cancellationToken);

        return new StudentAppInvitationResponse(invitation.Value.Id, invitation.Value.Email, invitation.Value.ExpiresAt);
    }
}
