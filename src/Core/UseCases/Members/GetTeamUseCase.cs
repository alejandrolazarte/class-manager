using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Members;

public sealed record GetTeamQuery : IQuery;

public sealed class GetTeamUseCase(
    IBusinessRepository businessRepository,
    IBusinessMemberRepository businessMemberRepository,
    IOrganizationMemberRepository organizationMemberRepository,
    IMemberInvitationRepository invitationRepository,
    IIdentityService identityService,
    ICurrentUser currentUser,
    IUserPhotoUrlService userPhotoUrls,
    TimeProvider timeProvider)
    : IUseCase<GetTeamQuery, TeamResponse>
{
    public async Task<Result<TeamResponse>> ExecuteAsync(GetTeamQuery command, CancellationToken cancellationToken)
    {
        var members = await businessMemberRepository.ListAsync(cancellationToken);
        var accounts = (await identityService.ListAccountsAsync([.. members.Select(member => member.User.AccountId).OfType<Guid>()], cancellationToken))
            .ToDictionary(account => account.UserId);
        var photoUrls = await userPhotoUrls.OfUsersAsync(members.Select(member => member.UserId), cancellationToken);
        var invitations = await invitationRepository.ListPendingAsync(timeProvider.GetUtcNow(), cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        var brandOwnerAccountIds = business is null
            ? []
            : (await organizationMemberRepository.ListBrandOwnerUserIdsAsync(business.OrganizationId, cancellationToken)).ToHashSet();

        return new TeamResponse(
            [
                .. members
                    .Select(member => MemberResponse.From(
                        member,
                        member.User.AccountId is { } accountId ? accounts.GetValueOrDefault(accountId) : null,
                        currentUser.AccountId,
                        member.User.AccountId is { } memberAccountId && brandOwnerAccountIds.Contains(memberAccountId),
                        photoUrls.GetValueOrDefault(member.UserId)))
                    .OrderBy(member => member.FullName, StringComparer.CurrentCultureIgnoreCase),
            ],
            [.. invitations.OrderBy(invitation => invitation.Email, StringComparer.OrdinalIgnoreCase).Select(InvitationResponse.From)]);
    }
}
