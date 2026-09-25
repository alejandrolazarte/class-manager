using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignIn_with_valid_credentials;

public sealed class Then_tokens_are_issued_for_member_business
{
    [Fact]
    public async Task Then_tokens_are_issued_for_member_business_Run()
    {
        var builder = new SignInUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(SignInUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.Tokens.Verify(
            service => service.IssueAsync(
                new SessionUser(builder.UserId, TestData.OwnerEmail, builder.Member.TenantId, BusinessRole.Owner),
                It.IsAny<CancellationToken>()),
            Times.Once);
        response.Value!.AccessToken.ShouldBe(TestData.IssuedTokens().AccessToken);
    }
}
