using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.U.Tests.UseCases.Members.When_current_member_is_resolved;

public sealed class Then_role_and_permissions_are_returned
{
    [Fact]
    public async Task Then_role_and_permissions_are_returned_Run()
    {
        var businessId = TestIds.Next();
        var currentMember = new Mock<ICurrentMember>();
        currentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(Guid.CreateVersion7(), TestIds.Next(), businessId, BusinessRole.BranchOwner, null, IsBrandOwner: true, SystemRolePermissions.BrandOwner));

        var featureAccess = new Mock<IFeatureAccess>();
        featureAccess
            .Setup(access => access.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(EffectiveFeatures.Inactive(string.Empty));

        var response = await new GetCurrentMemberUseCase(currentMember.Object, Mock.Of<IUserRepository>(), featureAccess.Object, Mock.Of<IFeatureUsage>(usage => usage.CountedFeatureCodes == Array.Empty<string>())).ExecuteAsync(new GetCurrentMemberQuery(), CancellationToken.None);

        response.Value.ShouldBe(
            new CurrentMemberResponse(businessId, BusinessRole.BranchOwner, null, IsBrandOwner: true, [.. Permissions.All.Order(StringComparer.Ordinal)]),
            new CurrentMemberResponseComparer());
    }

    private sealed class CurrentMemberResponseComparer : IEqualityComparer<CurrentMemberResponse?>
    {
        public bool Equals(CurrentMemberResponse? x, CurrentMemberResponse? y) =>
            x?.BusinessId == y?.BusinessId
            && x?.BranchRole == y?.BranchRole
            && x?.IsBrandOwner == y?.IsBrandOwner
            && (x?.Permissions ?? []).SequenceEqual(y?.Permissions ?? []);

        public int GetHashCode(CurrentMemberResponse? obj) => HashCode.Combine(obj?.BusinessId, obj?.BranchRole, obj?.IsBrandOwner);
    }
}
