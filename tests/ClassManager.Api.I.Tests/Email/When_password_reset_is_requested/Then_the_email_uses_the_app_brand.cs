using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.I.Tests.Email.When_password_reset_is_requested;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_uses_the_app_brand(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_uses_the_app_brand_Run()
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        await anonymous.SignUpAsync(ownerCommand);

        await anonymous.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailTransport, ownerCommand.Email!);

        var sentEmail = fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!)[^1];
        sentEmail.FromName.ShouldBeNull();
        sentEmail.HtmlBody.ShouldContain(BrandedEmailSender.AppDisplayName);
    }
}
