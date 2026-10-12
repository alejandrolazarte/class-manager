using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_short_password;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();
        var command = CompleteSignUpUseCaseBuilder.ValidCommand() with { Password = "too-short" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        response.Error.FieldName.ShouldBe(nameof(CompleteSignUpCommand.Password));
        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
