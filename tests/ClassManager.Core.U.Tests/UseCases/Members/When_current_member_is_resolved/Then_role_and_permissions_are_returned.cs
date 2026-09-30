using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Core.U.Tests.UseCases.Members.When_current_member_is_resolved;

public sealed class Then_role_and_permissions_are_returned
{
    [Fact]
    public async Task Then_role_and_permissions_are_returned_Run()
    {
        var businessId = Guid.CreateVersion7();
        var currentMember = new Mock<ICurrentMember>();
        currentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(Guid.CreateVersion7(), businessId, BusinessRole.BranchOwner, null, IsBrandOwner: true, SystemRolePermissions.BrandOwner));

        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var response = await new GetCurrentMemberUseCase(currentMember.Object, identityService.Object).ExecuteAsync(new GetCurrentMemberQuery(), CancellationToken.None);

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
