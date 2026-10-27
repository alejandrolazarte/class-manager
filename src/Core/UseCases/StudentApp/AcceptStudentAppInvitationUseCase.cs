using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Users;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record AcceptStudentAppInvitationCommand([property: AuditSecret] string? Token, [property: AuditPersonal] string? FullName, [property: AuditSecret] string? Password, bool AcceptsLegalTerms = false) : ICommand;

public sealed class AcceptStudentAppInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    IClientAccountRepository accountRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IUserRepository userRepository,
    IIdentityService identityService,
    IBusinessRepository businessRepository,
    ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository,
    ITokenService tokenService,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<AcceptStudentAppInvitationCommand, TokenResponse>
{
    private const string AlreadyLinkedMessage = "This account is already linked to another client of this school.";

    public async Task<Result<TokenResponse>> ExecuteAsync(AcceptStudentAppInvitationCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return InvalidInvitation();
        }

        var now = timeProvider.GetUtcNow();
        var invitation = await invitationRepository.FindForUpdateInAnyBusinessByTokenHashAsync(
            secretTokenGenerator.Hash(command.Token), cancellationToken);
        if (invitation is null || !invitation.IsPendingAt(now) || invitation.AwaitsGuardianConsent)
        {
            return InvalidInvitation();
        }

        tenantScope.Establish(invitation.TenantId);
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        var today = business?.TodayAt(now) ?? DateOnly.FromDateTime(now.UtcDateTime);
        var userId = await identityService.FindUserIdByEmailAsync(invitation.Email, cancellationToken);
        var invitedPerson = await InvitedPersonAsync(invitation, cancellationToken);
        ClientAccount? existingAccount = null;
        DateOnly? accountBirthDate;
        if (userId is null)
        {
            var account = OwnerAccount.CreateForInvitedStudent(
                invitedPerson?.FullName ?? command.FullName, invitation.Email, command.Password, invitedPerson?.BirthDate);
            if (account.IsFailure)
            {
                return account.Error!;
            }

            accountBirthDate = account.Value!.BirthDate;
            if (await ContactMinorErrorAsync(invitation, accountBirthDate, today, cancellationToken) is { } minorContact)
            {
                return minorContact;
            }

            if (!command.AcceptsLegalTerms)
            {
                return LegalTermsNotAccepted.For<TokenResponse>(nameof(AcceptStudentAppInvitationCommand.AcceptsLegalTerms));
            }

            if (business is not null
                && invitation.GuardianConsentedAt is null
                && accountBirthDate is { } birthDate
                && !PersonAge.CanHaveOwnAccount(birthDate, today, business.Country))
            {
                return Result.Validation<TokenResponse>(
                    AuthenticationErrorCodes.TooYoungForOwnAccountMessage,
                    AuthenticationErrorCodes.TooYoungForOwnAccount,
                    nameof(OwnerAccount.BirthDate));
            }

            var createdUserId = await identityService.CreateOwnerAsync(account.Value!, cancellationToken);
            if (createdUserId.IsFailure)
            {
                return createdUserId.Error!;
            }

            userId = createdUserId.Value;
            legalTermsAcceptanceRepository.Add(LegalTermsAcceptance.RecordCurrent(createdUserId.Value, now));
        }
        else
        {
            accountBirthDate = (await identityService.ListAccountsAsync([userId.Value], cancellationToken)).SingleOrDefault()?.BirthDate;
            if (await ContactMinorErrorAsync(invitation, accountBirthDate, today, cancellationToken) is { } minorContact)
            {
                return minorContact;
            }

            existingAccount = await accountRepository.FindByAccountAsync(userId.Value, cancellationToken);
        }

        if (existingAccount is not null && (existingAccount.ClientId != invitation.ClientId || existingAccount.StudentId != invitation.StudentId))
        {
            return Result.Conflict<TokenResponse>(AlreadyLinkedMessage, StudentAppErrorCodes.AlreadyLinked);
        }

        if (existingAccount is null)
        {
            var user = await AccountUsers.ResolveAsync(userRepository, identityService, invitedPerson, userId!.Value, now, cancellationToken);
            accountRepository.Add(ClientAccount.Create(invitation.ClientId, user, now, invitation.StudentId));
        }

        await CompleteContactBirthDateAsync(invitation, accountBirthDate, today, cancellationToken);
        invitation.Accept(now);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return InvalidInvitation();
        }

        var tokens = await tokenService.IssueAsync(
            new SessionUser(userId!.Value, invitation.Email, invitation.TenantId, AccountKinds.StudentRoleName, AccountKinds.Student),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TokenResponse.From(tokens);
    }

    private async Task<User?> InvitedPersonAsync(ClientInvitation invitation, CancellationToken cancellationToken) =>
        invitation.StudentId is { } studentId
            ? (await studentRepository.FindForUpdateAsync(studentId, cancellationToken))?.User
            : (await clientRepository.GetForUpdateAsync(invitation.ClientId, cancellationToken))?.User;

    private async Task<Client?> ClientWhoAttendsAsync(ClientInvitation invitation, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetForUpdateAsync(invitation.ClientId, cancellationToken);
        if (client is null)
        {
            return null;
        }

        var familyStudents = await studentRepository.ListByClientAsync(client.Id, cancellationToken);
        return familyStudents.Any(student => student.IsTheContactOf(client)) ? client : null;
    }

    private async Task<ResultError?> ContactMinorErrorAsync(ClientInvitation invitation, DateOnly? accountBirthDate, DateOnly today, CancellationToken cancellationToken)
    {
        if (invitation.StudentId is not null)
        {
            return null;
        }

        var birthDate = accountBirthDate ?? (await ClientWhoAttendsAsync(invitation, cancellationToken))?.User.BirthDate;
        return AttendingContact.MinorError(birthDate, today, nameof(OwnerAccount.BirthDate));
    }

    private async Task CompleteContactBirthDateAsync(ClientInvitation invitation, DateOnly? accountBirthDate, DateOnly today, CancellationToken cancellationToken)
    {
        if (invitation.StudentId is not null || accountBirthDate is null)
        {
            return;
        }

        var clientWhoAttends = await ClientWhoAttendsAsync(invitation, cancellationToken);
        if (clientWhoAttends is null || clientWhoAttends.User.BirthDate is not null)
        {
            return;
        }

        clientWhoAttends.User.ChangeBirthDate(accountBirthDate, today);
    }

    private static Result<TokenResponse> InvalidInvitation() =>
        Result.Validation<TokenResponse>(StudentAppErrorCodes.InvalidInvitationMessage, StudentAppErrorCodes.InvalidInvitation);
}
