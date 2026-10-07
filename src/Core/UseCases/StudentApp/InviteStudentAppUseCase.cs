using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;
using ClassManager.Notifications.Email;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record InviteStudentAppRequest(string? Email, Guid? StudentId = null, DateOnly? BirthDate = null)
{
    public InviteStudentAppCommand ToCommand(Guid clientId) => new(clientId, Email, StudentId, BirthDate);
}

public sealed record InviteStudentAppCommand(Guid ClientId, string? Email, Guid? StudentId = null, DateOnly? BirthDate = null) : ICommand;

public sealed class InviteStudentAppUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
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
    private const string StudentNotFoundMessage = "The student is not part of this client.";
    private const string BirthDateRequiredMessage = "Write the birth date of the person who will use the app.";

    public static EmailContent EmailContentFor(string businessName, string acceptInvitationLink) =>
        new(
            "Invitación a la app",
            EmailSubject,
            $"{businessName} te invita a su app para ver las clases, tus saldos y hacer pedidos. Tocá el botón para aceptar: si ya usás la app con este email, entrás con tu cuenta de siempre.",
            "Si no esperabas esta invitación, podés ignorar este mail.")
        {
            Action = new EmailAction("Aceptar invitación", acceptInvitationLink, ShowsLinkFallback: true),
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

        if (await clientAccountRepository.HasAccountAsync(client.Id, command.StudentId, cancellationToken))
        {
            return Result.Conflict<StudentAppInvitationResponse>(AlreadyUsesTheAppMessage, StudentAppErrorCodes.AlreadyUsesTheApp);
        }

        var now = timeProvider.GetUtcNow();
        var familyStudents = await studentRepository.ListByClientAsync(client.Id, cancellationToken);
        var recipient = command.StudentId is { } studentId
            ? await StudentRecipientAsync(client, studentId, familyStudents, command, business, business.TodayAt(now), cancellationToken)
            : ClientRecipient(client, familyStudents, command);
        if (recipient.IsFailure)
        {
            return recipient.Error!;
        }

        var token = secretTokenGenerator.Create();
        var invitation = ClientInvitation.Create(client.Id, recipient.Value, token.Hash, access.UserId, now, command.StudentId);
        if (invitation.IsFailure)
        {
            return invitation.Error!;
        }

        foreach (var previousInvitation in await invitationRepository.ListPendingForUpdateByPersonAsync(client.Id, command.StudentId, now, cancellationToken))
        {
            previousInvitation.Revoke(now);
        }

        invitationRepository.Add(invitation.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var content = EmailContentFor(business.BrandDisplayName, webAppLinks.AcceptStudentAppInvitation(token.Value));
        await emailSender.SendAsync(new EmailMessage(invitation.Value!.Email, EmailSubject, content, business.Id), cancellationToken);

        return new StudentAppInvitationResponse(invitation.Value.Id, invitation.Value.Email, invitation.Value.ExpiresAt);
    }

    private static Result<string> ClientRecipient(Client client, IReadOnlyList<Student> familyStudents, InviteStudentAppCommand command)
    {
        if (!string.IsNullOrWhiteSpace(command.Email) && !client.HasEmail(command.Email))
        {
            if (familyStudents.Any(student => student.HasEmail(command.Email)))
            {
                return FamilyEmails.EmailOfAnotherPerson(nameof(InviteStudentAppRequest.Email));
            }

            var emailChange = client.ChangeEmail(command.Email);
            if (emailChange.IsFailure)
            {
                return emailChange.Error! with { FieldName = nameof(InviteStudentAppRequest.Email) };
            }
        }

        return client.Email is null ? EmailRequired() : client.Email;
    }

    private async Task<Result<string>> StudentRecipientAsync(
        Client client,
        Guid studentId,
        IReadOnlyList<Student> familyStudents,
        InviteStudentAppCommand command,
        Business business,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var student = familyStudents.Any(familyStudent => familyStudent.Id == studentId)
            ? await studentRepository.FindForUpdateAsync(studentId, cancellationToken)
            : null;
        if (student is null)
        {
            return Result.NotFound<string>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
        }

        if (command.BirthDate is not null)
        {
            var birthDateChange = student.ChangeBirthDate(command.BirthDate, today);
            if (birthDateChange.IsFailure)
            {
                return birthDateChange.Error! with { FieldName = nameof(InviteStudentAppRequest.BirthDate) };
            }
        }

        if (student.BirthDate is not { } birthDate)
        {
            return Result.Validation<string>(BirthDateRequiredMessage, StudentAppErrorCodes.BirthDateRequired, nameof(InviteStudentAppRequest.BirthDate));
        }

        if (!PersonAge.CanHaveOwnAccount(birthDate, today, business.DefaultCountryCallingCode))
        {
            return Result.Validation<string>(
                AuthenticationErrorCodes.TooYoungForOwnAccountMessage,
                AuthenticationErrorCodes.TooYoungForOwnAccount,
                nameof(InviteStudentAppRequest.BirthDate));
        }

        if (!string.IsNullOrWhiteSpace(command.Email) && !student.HasEmail(command.Email))
        {
            var otherStudents = familyStudents.Where(familyStudent => familyStudent.Id != studentId);
            if (FamilyEmails.IsTakenByAnotherPerson(command.Email, client.Email, otherStudents))
            {
                return FamilyEmails.EmailOfAnotherPerson(nameof(InviteStudentAppRequest.Email));
            }

            var emailChange = student.ChangeEmail(command.Email);
            if (emailChange.IsFailure)
            {
                return emailChange.Error! with { FieldName = nameof(InviteStudentAppRequest.Email) };
            }
        }

        return student.Email is null ? EmailRequired() : student.Email;
    }

    private static Result<string> EmailRequired() =>
        Result.Validation<string>(EmailRequiredMessage, StudentAppErrorCodes.EmailRequired, nameof(InviteStudentAppRequest.Email));
}
