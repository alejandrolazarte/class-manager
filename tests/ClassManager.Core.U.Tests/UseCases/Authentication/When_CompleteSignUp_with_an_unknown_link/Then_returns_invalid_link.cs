using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_an_unknown_link;

public sealed class Then_returns_invalid_link
{
    [Fact]
    public async Task Then_returns_invalid_link_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();
        var command = CompleteSignUpUseCaseBuilder.ValidCommand() with { Token = "unknown-token" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidSignUpToken);
        builder.Businesses.Verify(repository => repository.Add(It.IsAny<Business>()), Times.Never);
    }
}
