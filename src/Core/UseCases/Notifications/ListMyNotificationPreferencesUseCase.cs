using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Notifications;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Notifications;

public sealed record ListMyNotificationPreferencesQuery(string? CurrentKind) : IQuery;

public sealed record NotificationPreferencesResponse(string BusinessName, IReadOnlyList<NotificationPreferenceResponse> Preferences);

public sealed class ListMyNotificationPreferencesUseCase(
    ICurrentUser currentUser,
    ITenantContext tenantContext,
    IBranchDirectory branchDirectory,
    IStudentAppDirectory studentAppDirectory,
    INotificationOptOutRepository optOutRepository)
    : IUseCase<ListMyNotificationPreferencesQuery, NotificationPreferencesResponse>
{
    public async Task<Result<NotificationPreferencesResponse>> ExecuteAsync(
        ListMyNotificationPreferencesQuery query, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return NoAccess();
        }

        var isStudentSession = AccountKinds.IsStudent(query.CurrentKind);
        var businessName = isStudentSession
            ? (await studentAppDirectory.FindAsync(accountId, tenantContext.TenantId, cancellationToken))?.BusinessName
            : (await branchDirectory.FindAsync(accountId, tenantContext.TenantId, cancellationToken))?.BusinessName;
        if (businessName is null)
        {
            return NoAccess();
        }

        var audience = isStudentSession ? NotificationAudience.Family : NotificationAudience.Team;
        var optOuts = await optOutRepository.ListAsync(accountId, cancellationToken);

        return new NotificationPreferencesResponse(
            businessName,
            [.. NotificationCategories.For(audience).Select(definition => NotificationPreferenceResponse.From(definition, optOuts))]);
    }

    private static Result<NotificationPreferencesResponse> NoAccess() =>
        Result.Unauthorized<NotificationPreferencesResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
}
