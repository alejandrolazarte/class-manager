using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.Emails;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record InviteStudentAppRequest(
    string? Email,
    Guid? StudentId = null,
    DateOnly? BirthDate = null,
    bool ConfirmsContactIsAdult = false,
    Guid? SamePersonUserId = null)
{
    public InviteStudentAppCommand ToCommand(Guid clientId) => new(clientId, Email, StudentId, BirthDate, ConfirmsContactIsAdult, SamePersonUserId);
}

public sealed record InviteStudentAppCommand(
    Guid ClientId,
    [property: AuditPersonal] string? Email,
    Guid? StudentId = null,
    [property: AuditPersonal] DateOnly? BirthDate = null,
    bool ConfirmsContactIsAdult = false,
    Guid? SamePersonUserId = null) : ICommand;

public sealed class InviteStudentAppUseCase(
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IBusinessRepository businessRepository,
    IClientInvitationRepository invitationRepository,
    IClientAccountRepository clientAccountRepository,
    IUserRepository userRepository,
    IPeopleLock peopleLock,
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
    private const string ClientNotFoundMessage = "The client does not exist.";
    private const string EmailRequiredMessage = "Write the email of the person who will use the app.";
    private const string AlreadyUsesTheAppMessage = "The client already uses the app.";
    private const string StudentNotFoundMessage = "The student is not part of this client.";
    private const string GuardianEmailRequiredMessage = "Write the email of the client: a student this young needs their authorization.";
    private const string BirthDateRequiredMessage = "Write the birth date of the person who will use the app.";
    private const string ContactAdultConfirmationRequiredMessage = "Confirm that the contact is 18 or older: the contact pays for the classes.";

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
        await peopleLock.LockAsync(cancellationToken);
        var familyStudents = await studentRepository.ListByClientForUpdateAsync(client.Id, cancellationToken);
        var recipient = command.StudentId is { } studentId
            ? StudentRecipient(client, studentId, familyStudents, command, business, business.TodayAt(now))
            : ClientRecipient(client, familyStudents, command, business.TodayAt(now));
        if (recipient.IsFailure)
        {
            return recipient.Error!;
        }

        var invitedPerson = command.StudentId is { } invitedStudentId ? familyStudents.Single(student => student.Id == invitedStudentId).User : client.User;
        var samePerson = await SamePerson.FindAsync(
            userRepository,
            invitedPerson,
            invitedPerson.Email,
            command.SamePersonUserId,
            roles => !roles.IsClient && !roles.IsStudent,
            nameof(InviteStudentAppRequest.Email),
            cancellationToken);
        if (samePerson.IsFailure)
        {
            return samePerson.Error!;
        }

        if (samePerson.Value is { } person)
        {
            await MoveToAsync(person, invitedPerson, client, familyStudents, cancellationToken);
        }

        var previousInvitations = await invitationRepository.ListPendingForUpdateByPersonAsync(client.Id, command.StudentId, now, cancellationToken);
        var confirmedAdultInvitation = previousInvitations.FirstOrDefault(previousInvitation => previousInvitation.ContactAdultConfirmedAt is not null);
        if (command.StudentId is null && !command.ConfirmsContactIsAdult && confirmedAdultInvitation is null)
        {
            return Result.Validation<StudentAppInvitationResponse>(
                ContactAdultConfirmationRequiredMessage,
                StudentAppErrorCodes.ContactAdultConfirmationRequired,
                nameof(InviteStudentAppRequest.ConfirmsContactIsAdult));
        }

        var token = secretTokenGenerator.Create();
        var invitation = ClientInvitation.Create(client.Id, recipient.Value!.Email, token.Hash, access.UserId, now, command.StudentId);
        if (invitation.IsFailure)
        {
            return invitation.Error!;
        }

        if (command.StudentId is null && command.ConfirmsContactIsAdult)
        {
            invitation.Value!.ConfirmContactIsAdult(access.UserId, now);
        }
        else if (command.StudentId is null)
        {
            invitation.Value!.KeepContactAdultConfirmationOf(confirmedAdultInvitation!);
        }

        var guardianToken = recipient.Value.NeedsGuardianConsent ? secretTokenGenerator.Create() : null;
        if (guardianToken is not null)
        {
            invitation.Value!.RequireGuardianConsent(client.User.Email!, guardianToken.Hash);
        }

        foreach (var previousInvitation in previousInvitations)
        {
            previousInvitation.Revoke(now);
        }

        invitationRepository.Add(invitation.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (guardianToken is not null)
        {
            var guardianConsentRequest = EmailTexts.Spanish.GuardianConsentRequest(
                business.BrandDisplayName,
                recipient.Value.StudentFullName ?? string.Empty,
                business.Country.OwnAccountMinimumAge,
                webAppLinks.AuthorizeStudentApp(guardianToken.Value));
            await emailSender.SendAsync(
                new EmailMessage(client.User.Email!, guardianConsentRequest.Subject, guardianConsentRequest.Content, business.Id),
                cancellationToken);
            await studentAppNotificationService.GuardianConsentRequestedAsync(invitation.Value!, recipient.Value.StudentFullName ?? string.Empty, cancellationToken);
        }
        else
        {
            var studentAppInvitation = EmailTexts.Spanish.StudentAppInvitation(business.BrandDisplayName, webAppLinks.AcceptStudentAppInvitation(token.Value));
            await emailSender.SendAsync(
                new EmailMessage(invitation.Value!.Email, studentAppInvitation.Subject, studentAppInvitation.Content, business.Id),
                cancellationToken);
        }

        return new StudentAppInvitationResponse(invitation.Value!.Id, invitation.Value.Email, invitation.Value.ExpiresAt, invitation.Value.AwaitsGuardianConsent);
    }

    private async Task MoveToAsync(
        User person,
        User previousPerson,
        Client client,
        IReadOnlyList<Student> familyStudents,
        CancellationToken cancellationToken)
    {
        if (client.UserId == previousPerson.Id)
        {
            client.ChangePerson(person);
        }

        foreach (var student in familyStudents.Where(student => student.UserId == previousPerson.Id))
        {
            student.ChangePerson(person);
        }

        person.CompleteWith(previousPerson);
        await SamePerson.RemoveIfUnusedAsync(
            userRepository, previousPerson, roles => roles.IsInstructor || roles.IsTeamMember, cancellationToken);
    }

    private static Result<Recipient> ClientRecipient(
        Client client,
        IReadOnlyList<Student> familyStudents,
        InviteStudentAppCommand command,
        DateOnly today)
    {
        var minorContact = AttendingContactMinorError(client, familyStudents, command.BirthDate, today);
        if (minorContact is not null)
        {
            return minorContact;
        }

        if (!string.IsNullOrWhiteSpace(command.Email) && !client.User.HasEmail(command.Email))
        {
            var emailChange = client.User.ChangeEmail(command.Email);
            if (emailChange.IsFailure)
            {
                return emailChange.Error! with { FieldName = nameof(InviteStudentAppRequest.Email) };
            }

            if (FamilyEmails.IsTakenByAnotherPerson(client.User, familyStudents.Select(student => student.User)))
            {
                return FamilyEmails.EmailOfAnotherPerson(nameof(InviteStudentAppRequest.Email));
            }
        }

        return client.User.Email is null ? EmailRequired() : new Recipient(client.User.Email, NeedsGuardianConsent: false, StudentFullName: null);
    }

    private static ResultError? AttendingContactMinorError(
        Client client,
        IReadOnlyList<Student> familyStudents,
        DateOnly? typedBirthDate,
        DateOnly today)
    {
        if (!familyStudents.Any(student => student.IsTheContactOf(client)))
        {
            return null;
        }

        if (typedBirthDate is not null)
        {
            var birthDateChange = client.User.ChangeBirthDate(typedBirthDate, today);
            if (birthDateChange.IsFailure)
            {
                return birthDateChange.Error! with { FieldName = nameof(InviteStudentAppRequest.BirthDate) };
            }
        }

        return AttendingContact.MinorError(client.User.BirthDate, today, nameof(InviteStudentAppRequest.BirthDate));
    }

    private static Result<Recipient> StudentRecipient(
        Client client,
        Guid studentId,
        IReadOnlyList<Student> familyStudents,
        InviteStudentAppCommand command,
        Business business,
        DateOnly today)
    {
        var student = familyStudents.FirstOrDefault(familyStudent => familyStudent.Id == studentId);
        if (student is null)
        {
            return Result.NotFound<Recipient>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
        }

        if (command.BirthDate is not null)
        {
            var birthDateChange = student.User.ChangeBirthDate(command.BirthDate, today);
            if (birthDateChange.IsFailure)
            {
                return birthDateChange.Error! with { FieldName = nameof(InviteStudentAppRequest.BirthDate) };
            }
        }

        if (student.User.BirthDate is not { } birthDate)
        {
            return Result.Validation<Recipient>(BirthDateRequiredMessage, StudentAppErrorCodes.BirthDateRequired, nameof(InviteStudentAppRequest.BirthDate));
        }

        var needsGuardianConsent = !PersonAge.CanHaveOwnAccount(birthDate, today, business.Country);
        if (needsGuardianConsent && client.User.Email is null)
        {
            return Result.Validation<Recipient>(
                GuardianEmailRequiredMessage,
                StudentAppErrorCodes.GuardianEmailRequired,
                nameof(InviteStudentAppRequest.Email));
        }

        if (!string.IsNullOrWhiteSpace(command.Email) && !student.User.HasEmail(command.Email))
        {
            var emailChange = student.User.ChangeEmail(command.Email);
            if (emailChange.IsFailure)
            {
                return emailChange.Error! with { FieldName = nameof(InviteStudentAppRequest.Email) };
            }

            if (FamilyEmails.IsTakenByAnotherPerson(student.User, [client.User, .. familyStudents.Select(familyStudent => familyStudent.User)]))
            {
                return FamilyEmails.EmailOfAnotherPerson(nameof(InviteStudentAppRequest.Email));
            }
        }

        return student.User.Email is null ? EmailRequired() : new Recipient(student.User.Email, needsGuardianConsent, student.User.FullName);
    }

    private static Result<Recipient> EmailRequired() =>
        Result.Validation<Recipient>(EmailRequiredMessage, StudentAppErrorCodes.EmailRequired, nameof(InviteStudentAppRequest.Email));

    private sealed record Recipient(string Email, bool NeedsGuardianConsent, string? StudentFullName);
}
