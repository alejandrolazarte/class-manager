using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_without_accepting_the_legal_terms;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();
        var command = RequestSignUpUseCaseBuilder.ValidCommand() with { AcceptsLegalTerms = false };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.LegalTermsNotAccepted);
        response.Error.FieldName.ShouldBe(nameof(RequestSignUpCommand.AcceptsLegalTerms));
        builder.PendingSignUps.Verify(repository => repository.Add(It.IsAny<PendingSignUp>()), Times.Never);
    }
}
