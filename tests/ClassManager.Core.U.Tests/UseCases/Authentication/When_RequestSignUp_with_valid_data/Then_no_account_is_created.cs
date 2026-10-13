using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_with_valid_data;

public sealed class Then_no_account_is_created
{
    [Fact]
    public async Task Then_no_account_is_created_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(RequestSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.Identity.Verify(
            service => service.CreateOwnerAsync(It.IsAny<OwnerAccount>(), It.IsAny<LegalTermsAcceptance?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
