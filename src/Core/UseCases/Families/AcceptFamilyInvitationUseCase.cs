using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Families;

public sealed record AcceptFamilyInvitationCommand(string? Token, string? FullName, string? Password);

public sealed class AcceptFamilyInvitationUseCase(
    IClientInvitationRepository invitationRepository,
    IClientAccountRepository accountRepository,
    IIdentityService identityService,
    ITokenService tokenService,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<AcceptFamilyInvitationCommand, TokenResponse>
{
    private const string AlreadyLinkedMessage = "This account already belongs to another family of this school.";

    public async Task<Result<TokenResponse>> ExecuteAsync(AcceptFamilyInvitationCommand command, CancellationToken cancellationToken)
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
            var account = OwnerAccount.Create(command.FullName, invitation.Email, command.Password);
            if (account.IsFailure)
            {
                return account.Error!;
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

        if (existingAccount is not null && existingAccount.ClientId != invitation.ClientId)
        {
            return Result.Conflict<TokenResponse>(AlreadyLinkedMessage, FamilyErrorCodes.AlreadyLinked);
        }

        if (existingAccount is null)
        {
            accountRepository.Add(ClientAccount.Create(invitation.ClientId, userId!.Value, now));
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
            new SessionUser(userId!.Value, invitation.Email, invitation.TenantId, AccountKinds.FamilyRoleName, AccountKinds.Family),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TokenResponse.From(tokens);
    }

    private static Result<TokenResponse> InvalidInvitation() =>
        Result.Validation<TokenResponse>(FamilyErrorCodes.InvalidInvitationMessage, FamilyErrorCodes.InvalidInvitation);
}
