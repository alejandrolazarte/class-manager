using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.UseCases.Notifications;

public sealed record ListMyNotificationPreferencesQuery : IQuery;

public sealed class ListMyNotificationPreferencesUseCase(
    ICurrentUser currentUser,
    IBranchDirectory branchDirectory,
    IStudentAppDirectory studentAppDirectory,
    INotificationOptOutRepository optOutRepository)
    : IUseCase<ListMyNotificationPreferencesQuery, IReadOnlyList<NotificationPreferenceResponse>>
{
    public async Task<Result<IReadOnlyList<NotificationPreferenceResponse>>> ExecuteAsync(
        ListMyNotificationPreferencesQuery query, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return Result.Unauthorized<IReadOnlyList<NotificationPreferenceResponse>>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var isTeamMember = (await branchDirectory.ListAsync(accountId, cancellationToken)).Count > 0;
        var hasFamilyAccess = (await studentAppDirectory.ListAsync(accountId, cancellationToken)).Count > 0;
        var optOuts = await optOutRepository.ListAsync(accountId, cancellationToken);

        return Result.Success<IReadOnlyList<NotificationPreferenceResponse>>(
        [
            .. NotificationCategories.ApplyingTo(isTeamMember, hasFamilyAccess)
                .Select(definition => NotificationPreferenceResponse.From(definition, optOuts)),
        ]);
    }
}
