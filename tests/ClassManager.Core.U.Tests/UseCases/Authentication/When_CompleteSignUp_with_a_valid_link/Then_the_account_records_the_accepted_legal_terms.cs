using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_CompleteSignUp_with_a_valid_link;

public sealed class Then_the_account_records_the_accepted_legal_terms
{
    [Fact]
    public async Task Then_the_account_records_the_accepted_legal_terms_Run()
    {
        var builder = new CompleteSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(CompleteSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        var acceptance = builder.AddedLegalTermsAcceptance!;
        acceptance.UserId.ShouldBe(builder.UserId);
        acceptance.Version.ShouldBe(LegalTerms.CurrentVersion);
        acceptance.AcceptedOn.ShouldBe(TestData.Now);
    }
}
