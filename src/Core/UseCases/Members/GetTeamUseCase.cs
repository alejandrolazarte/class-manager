using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Members;

public sealed record GetTeamQuery;

public sealed class GetTeamUseCase(
    IBusinessMemberRepository businessMemberRepository,
    IMemberInvitationRepository invitationRepository,
    IIdentityService identityService,
    ICurrentUser currentUser,
    TimeProvider timeProvider)
    : IUseCase<GetTeamQuery, TeamResponse>
{
    public async Task<Result<TeamResponse>> ExecuteAsync(GetTeamQuery command, CancellationToken cancellationToken)
    {
        var members = await businessMemberRepository.ListAsync(cancellationToken);
        var accounts = (await identityService.ListAccountsAsync([.. members.Select(member => member.UserId)], cancellationToken))
            .ToDictionary(account => account.UserId);
        var invitations = await invitationRepository.ListPendingAsync(timeProvider.GetUtcNow(), cancellationToken);

        return new TeamResponse(
            [
                .. members
                    .Select(member => MemberResponse.From(member, accounts.GetValueOrDefault(member.UserId), currentUser.UserId))
                    .OrderBy(member => member.FullName, StringComparer.CurrentCultureIgnoreCase),
            ],
            [.. invitations.OrderBy(invitation => invitation.Email, StringComparer.OrdinalIgnoreCase).Select(InvitationResponse.From)]);
    }
}
