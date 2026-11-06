namespace ClassManager.Api.I.Tests.Email.When_password_reset_is_requested;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_cannot_be_unsubscribed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_cannot_be_unsubscribed_Run()
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, ownerCommand);

        await anonymous.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailTransport, ownerCommand.Email!);

        var sentEmail = fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!)[^1];
        sentEmail.OneClickUnsubscribeUrl.ShouldBeNull();
        sentEmail.HtmlBody.ShouldNotContain("/unsubscribe");
    }
}
