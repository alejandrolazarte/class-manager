using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Members;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.U.Tests.UseCases.Members.When_current_user_has_no_access;

public sealed class Then_result_is_unauthorized
{
    [Fact]
    public async Task Then_result_is_unauthorized_Run()
    {
        var currentMember = new Mock<ICurrentMember>();
        currentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((MemberAccess?)null);

        var response = await new GetCurrentMemberUseCase(currentMember.Object, Mock.Of<IIdentityService>(), Mock.Of<IFeatureAccess>(), Mock.Of<IFeatureUsage>()).ExecuteAsync(new GetCurrentMemberQuery(), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Unauthorized);
    }
}
