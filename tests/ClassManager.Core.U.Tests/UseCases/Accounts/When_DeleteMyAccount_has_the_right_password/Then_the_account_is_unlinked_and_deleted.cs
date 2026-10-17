using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_DeleteMyAccount_has_the_right_password;

public sealed class Then_the_account_is_unlinked_and_deleted
{
    [Fact]
    public async Task Then_the_account_is_unlinked_and_deleted_Run()
    {
        var builder = new DeleteMyAccountUseCaseBuilder().WithCorrectPassword();

        var response = await builder.Build().ExecuteAsync(
            new DeleteMyAccountCommand(DeleteMyAccountUseCaseBuilder.CurrentPassword), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.AccountLinks.Verify(repository => repository.UnlinkFromEveryBusinessAsync(
            builder.AccountId, DeleteMyAccountUseCaseBuilder.Now, It.IsAny<CancellationToken>()));
        builder.Identity.Verify(service => service.DeleteAccountAsync(builder.AccountId, It.IsAny<CancellationToken>()));
    }
}
