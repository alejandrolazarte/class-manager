using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_by_a_minor;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();
        var command = RequestSignUpUseCaseBuilder.ValidCommand() with { OwnerBirthDate = TestData.Today.AddYears(-16) };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.OwnerMustBeAdult);
        response.Error.FieldName.ShouldBe(nameof(RequestSignUpCommand.OwnerBirthDate));
        builder.PendingSignUps.Verify(repository => repository.Add(It.IsAny<PendingSignUp>()), Times.Never);
    }
}
