using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Members;

public sealed record AcceptInvitationCommand(string? Token, string? FullName, string? Password);

public sealed class AcceptInvitationUseCase(
    IMemberInvitationRepository invitationRepository,
    IBusinessMemberRepository businessMemberRepository,
    ICustomRoleRepository customRoleRepository,
    IIdentityService identityService,
    ITokenService tokenService,
    ISecretTokenGenerator secretTokenGenerator,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<AcceptInvitationCommand, TokenResponse>
{
    private const string InvalidInvitationMessage = "The invitation link is invalid, expired or already used. Ask for a new one.";
    private const string AlreadyMemberMessage = "You are already part of this team.";
    private const string InstructorTakenMessage = "Another team member is already linked to this coach. Ask for a new invitation.";

    public async Task<Result<TokenResponse>> ExecuteAsync(AcceptInvitationCommand command, CancellationToken cancellationToken)
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
        var role = await MemberRules.ResolveRoleAsync(customRoleRepository, invitation.Role, invitation.CustomRoleId, cancellationToken);
        if (role.IsFailure)
        {
            return InvalidInvitation();
        }

        if (invitation.InstructorId is { } instructorId
            && await businessMemberRepository.IsInstructorLinkedAsync(instructorId, null, cancellationToken))
        {
            return Result.Conflict<TokenResponse>(InstructorTakenMessage, MemberErrorCodes.InstructorTaken);
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var userId = await identityService.FindUserIdByEmailAsync(invitation.Email, cancellationToken);
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
        else if (await businessMemberRepository.IsUserMemberAsync(userId.Value, cancellationToken))
        {
            return Result.Conflict<TokenResponse>(AlreadyMemberMessage, MemberErrorCodes.AlreadyMember);
        }

        var member = BusinessMember.Create(invitation.TenantId, userId.Value, role.Value!, invitation.InstructorId);
        if (member.IsFailure)
        {
            return member.Error!;
        }

        businessMemberRepository.Add(member.Value!);
        invitation.Accept(now);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = await tokenService.IssueAsync(
            new SessionUser(userId.Value, invitation.Email, invitation.TenantId, invitation.Role.ToString()),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TokenResponse.From(tokens);
    }

    private static Result<TokenResponse> InvalidInvitation() =>
        Result.Validation<TokenResponse>(InvalidInvitationMessage, MemberErrorCodes.InvalidInvitation);
}
