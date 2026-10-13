using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_with_valid_data;

public sealed class Then_the_legal_terms_acceptance_waits_with_the_sign_up
{
    [Fact]
    public async Task Then_the_legal_terms_acceptance_waits_with_the_sign_up_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(RequestSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.AddedPendingSignUp!.LegalTermsAcceptance.ShouldBe(new LegalTermsAcceptance(LegalTerms.CurrentVersion, TestData.Now));
    }
}
