using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Email.When_every_category_is_turned_off;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_password_reset_is_still_sent(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_password_reset_is_still_sent_Run()
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerTokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, ownerCommand);
        using var owner = fixture.CreateClientWithToken(ownerTokens.AccessToken);
        foreach (var definition in NotificationCategories.All.Where(definition => definition.Uses(NotificationChannel.Email)))
        {
            await owner.TurnOffAsync(definition.Category, NotificationChannel.Email);
        }

        var emailsBefore = fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!).Count;
        await anonymous.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailTransport, ownerCommand.Email!);

        fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!).Count.ShouldBe(emailsBefore + 1);
    }
}
