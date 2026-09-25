using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_short_password;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();
        var command = SignUpOwnerUseCaseBuilder.ValidCommand() with { Password = "too-short" };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        response.Error.FieldName.ShouldBe(nameof(SignUpOwnerCommand.Password));
        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
