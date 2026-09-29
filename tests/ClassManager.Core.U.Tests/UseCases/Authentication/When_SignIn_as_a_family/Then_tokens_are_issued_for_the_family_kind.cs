using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_as_a_family;

public sealed class Then_tokens_are_issued_for_the_family_kind
{
    [Fact]
    public async Task Then_tokens_are_issued_for_the_family_kind_Run()
    {
        var builder = new SignInUseCaseBuilder();
        builder.WithoutBranch();
        builder.WithFamily();

        var response = await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Tokens.Verify(
            service => service.IssueAsync(
                new SessionUser(builder.UserId, TestData.OwnerEmail, builder.Family.BusinessId, AccountKinds.FamilyRoleName, AccountKinds.Family),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
