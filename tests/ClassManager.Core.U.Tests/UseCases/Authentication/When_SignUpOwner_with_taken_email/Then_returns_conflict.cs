using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_SignUpOwner_with_taken_email;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new SignUpOwnerUseCaseBuilder();
        builder.Identity
            .Setup(service => service.IsEmailRegisteredAsync(TestData.OwnerEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await builder.Build().ExecuteAsync(SignUpOwnerUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.EmailTaken);
        response.Error.Kind.ShouldBe(ErrorKind.Conflict);
        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
