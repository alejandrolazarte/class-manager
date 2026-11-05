using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_DeleteMyAccount_has_the_right_password;

public sealed class Then_its_notification_choices_are_deleted
{
    [Fact]
    public async Task Then_its_notification_choices_are_deleted_Run()
    {
        var builder = new DeleteMyAccountUseCaseBuilder().WithCorrectPassword();

        await builder.Build().ExecuteAsync(new DeleteMyAccountCommand(DeleteMyAccountUseCaseBuilder.CurrentPassword), CancellationToken.None);

        builder.NotificationOptOuts.Verify(repository => repository.DeleteEveryOptOutOfAccountAsync(builder.AccountId, It.IsAny<CancellationToken>()));
    }
}
