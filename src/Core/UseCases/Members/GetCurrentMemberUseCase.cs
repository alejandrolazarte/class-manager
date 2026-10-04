using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.Members;

public sealed record GetCurrentMemberQuery : IQuery;

public sealed record CurrentMemberResponse(
    Guid BusinessId,
    BusinessRole? BranchRole,
    Guid? InstructorId,
    bool IsBrandOwner,
    IReadOnlyList<string> Permissions,
    Guid? CustomRoleId = null,
    Guid? UserId = null,
    string? FullName = null,
    CurrentSubscriptionResponse? Subscription = null)
{
    public static CurrentMemberResponse From(MemberAccess access, string? fullName = null, CurrentSubscriptionResponse? subscription = null) =>
        new(access.BusinessId, access.BranchRole, access.InstructorId, access.IsBrandOwner, [.. access.Permissions.Order(StringComparer.Ordinal)], access.CustomRoleId, access.UserId, fullName, subscription);
}

public sealed class GetCurrentMemberUseCase(
    ICurrentMember currentMember,
    IIdentityService identityService,
    IFeatureAccess featureAccess,
    IFeatureUsage featureUsage)
    : IUseCase<GetCurrentMemberQuery, CurrentMemberResponse>
{
    public async Task<Result<CurrentMemberResponse>> ExecuteAsync(GetCurrentMemberQuery command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return Result.Unauthorized<CurrentMemberResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var accounts = await identityService.ListAccountsAsync([access.UserId], cancellationToken);
        var features = await featureAccess.GetCurrentAsync(cancellationToken);
        var usageByFeatureCode = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var featureCode in featureUsage.CountedFeatureCodes.Where(features.Has))
        {
            usageByFeatureCode[featureCode] = await featureUsage.CountAsync(featureCode, cancellationToken);
        }

        return CurrentMemberResponse.From(
            access,
            accounts.Count > 0 ? accounts[0].FullName : null,
            CurrentSubscriptionResponse.From(features, usageByFeatureCode));
    }
}
