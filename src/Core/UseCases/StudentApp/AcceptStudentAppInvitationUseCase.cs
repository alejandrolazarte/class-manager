using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record AcceptStudentAppInvitationCommand(string? Token, string? FullName, string? Password, DateOnly? BirthDate = null) : ICommand;

public sealed class AcceptStudentAppInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    IClientAccountRepository accountRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IIdentityService identityService,
    IBusinessRepository businessRepository,
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
        if (invitation is null || !invitation.IsPendingAt(now))
        {
            return InvalidInvitation();
        }

        tenantScope.Establish(invitation.TenantId);
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var userId = await identityService.FindUserIdByEmailAsync(invitation.Email, cancellationToken);
        ClientAccount? existingAccount = null;
        if (userId is null)
        {
            var invitedName = (await InvitedStudent.FindAsync(invitation, clientRepository, studentRepository, cancellationToken)).FullName;
            var business = await businessRepository.GetCurrentAsync(cancellationToken);
            var today = business?.TodayAt(now) ?? DateOnly.FromDateTime(now.UtcDateTime);
            var account = OwnerAccount.Create(invitedName ?? command.FullName, invitation.Email, command.Password, command.BirthDate, today);
            if (account.IsFailure)
            {
                return account.Error!;
            }

            if (business is not null && !PersonAge.CanHaveOwnAccount(account.Value!.BirthDate, today, business.DefaultCountryCallingCode))
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
        }
        else
        {
            existingAccount = await accountRepository.FindByUserAsync(userId.Value, cancellationToken);
        }

        if (existingAccount is not null && (existingAccount.ClientId != invitation.ClientId || existingAccount.StudentId != invitation.StudentId))
        {
            return Result.Conflict<TokenResponse>(AlreadyLinkedMessage, StudentAppErrorCodes.AlreadyLinked);
        }

        if (existingAccount is null)
        {
            accountRepository.Add(ClientAccount.Create(invitation.ClientId, userId!.Value, now, invitation.StudentId));
        }

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

    private static Result<TokenResponse> InvalidInvitation() =>
        Result.Validation<TokenResponse>(StudentAppErrorCodes.InvalidInvitationMessage, StudentAppErrorCodes.InvalidInvitation);
}
