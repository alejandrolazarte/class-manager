using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Clients;
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
    IStudentAppNotificationService studentAppNotificationService,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<InviteStudentAppCommand, StudentAppInvitationResponse>
{
    public const string EmailSubject = "Te invitaron a la app";
    public const string GuardianEmailSubject = "Pedido de autorización para la app";

    private const string ClientNotFoundMessage = "The client does not exist.";
    private const string EmailRequiredMessage = "Write the email of the person who will use the app.";
    private const string AlreadyUsesTheAppMessage = "The client already uses the app.";
    private const string StudentNotFoundMessage = "The student is not part of this client.";
    private const string GuardianEmailRequiredMessage = "Write the email of the client: a student this young needs their authorization.";
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

    public static EmailContent GuardianEmailContentFor(string businessName, string studentFullName, int minimumAge, string authorizeLink) =>
        new(
            "Autorización para la app",
            GuardianEmailSubject,
            $"{businessName} quiere invitar a {studentFullName} a su app para ver sus clases. Como es menor de {minimumAge} años, necesitamos tu autorización.",
            "Si no esperabas este pedido, podés ignorar este mail.")
        {
            Action = new EmailAction("Revisar y autorizar", authorizeLink, ShowsLinkFallback: true),
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
            : await ClientRecipientAsync(client, familyStudents, command, business.TodayAt(now), cancellationToken);
        if (recipient.IsFailure)
        {
            return recipient.Error!;
        }

        var token = secretTokenGenerator.Create();
        var invitation = ClientInvitation.Create(client.Id, recipient.Value!.Email, token.Hash, access.UserId, now, command.StudentId);
        if (invitation.IsFailure)
        {
            return invitation.Error!;
        }

        var guardianToken = recipient.Value.NeedsGuardianConsent ? secretTokenGenerator.Create() : null;
        if (guardianToken is not null)
        {
            invitation.Value!.RequireGuardianConsent(client.Email!, guardianToken.Hash);
        }

        foreach (var previousInvitation in await invitationRepository.ListPendingForUpdateByPersonAsync(client.Id, command.StudentId, now, cancellationToken))
        {
            previousInvitation.Revoke(now);
        }

        invitationRepository.Add(invitation.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (guardianToken is not null)
        {
            var guardianContent = GuardianEmailContentFor(
                business.BrandDisplayName,
                recipient.Value.StudentFullName ?? string.Empty,
                PersonAge.OwnAccountMinimumAge(business.DefaultCountryCallingCode),
                webAppLinks.AuthorizeStudentApp(guardianToken.Value));
            await emailSender.SendAsync(new EmailMessage(client.Email!, GuardianEmailSubject, guardianContent, business.Id), cancellationToken);
            await studentAppNotificationService.GuardianConsentRequestedAsync(invitation.Value!, recipient.Value.StudentFullName ?? string.Empty, cancellationToken);
        }
        else
        {
            var content = EmailContentFor(business.BrandDisplayName, webAppLinks.AcceptStudentAppInvitation(token.Value));
            await emailSender.SendAsync(new EmailMessage(invitation.Value!.Email, EmailSubject, content, business.Id), cancellationToken);
        }

        return new StudentAppInvitationResponse(invitation.Value!.Id, invitation.Value.Email, invitation.Value.ExpiresAt, invitation.Value.AwaitsGuardianConsent);
    }

    private async Task<Result<Recipient>> ClientRecipientAsync(
        Client client,
        IReadOnlyList<Student> familyStudents,
        InviteStudentAppCommand command,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var minorContact = await AttendingContactMinorErrorAsync(client, familyStudents, command.BirthDate, today, cancellationToken);
        if (minorContact is not null)
        {
            return minorContact;
        }

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

        return client.Email is null ? EmailRequired() : new Recipient(client.Email, NeedsGuardianConsent: false, StudentFullName: null);
    }

    private async Task<ResultError?> AttendingContactMinorErrorAsync(
        Client client,
        IReadOnlyList<Student> familyStudents,
        DateOnly? typedBirthDate,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var contactAsStudent = familyStudents.FirstOrDefault(student => AttendingContact.IsTheContact(client.FullName, student.FullName));
        if (contactAsStudent is null)
        {
            return null;
        }

        if (typedBirthDate is not null)
        {
            var trackedStudent = await studentRepository.FindForUpdateAsync(contactAsStudent.Id, cancellationToken);
            var birthDateChange = trackedStudent?.ChangeBirthDate(typedBirthDate, today);
            if (birthDateChange?.IsFailure == true)
            {
                return birthDateChange.Error! with { FieldName = nameof(InviteStudentAppRequest.BirthDate) };
            }
        }

        return AttendingContact.MinorError(typedBirthDate ?? contactAsStudent.BirthDate, today, nameof(InviteStudentAppRequest.BirthDate));
    }

    private async Task<Result<Recipient>> StudentRecipientAsync(
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
            return Result.NotFound<Recipient>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
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
            return Result.Validation<Recipient>(BirthDateRequiredMessage, StudentAppErrorCodes.BirthDateRequired, nameof(InviteStudentAppRequest.BirthDate));
        }

        var needsGuardianConsent = !PersonAge.CanHaveOwnAccount(birthDate, today, business.DefaultCountryCallingCode);
        if (needsGuardianConsent && client.Email is null)
        {
            return Result.Validation<Recipient>(
                GuardianEmailRequiredMessage,
                StudentAppErrorCodes.GuardianEmailRequired,
                nameof(InviteStudentAppRequest.Email));
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

        return student.Email is null ? EmailRequired() : new Recipient(student.Email, needsGuardianConsent, student.FullName);
    }

    private static Result<Recipient> EmailRequired() =>
        Result.Validation<Recipient>(EmailRequiredMessage, StudentAppErrorCodes.EmailRequired, nameof(InviteStudentAppRequest.Email));

    private sealed record Recipient(string Email, bool NeedsGuardianConsent, string? StudentFullName);
}
