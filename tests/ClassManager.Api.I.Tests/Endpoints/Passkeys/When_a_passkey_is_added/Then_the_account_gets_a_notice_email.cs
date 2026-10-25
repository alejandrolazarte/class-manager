namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_passkey_is_added;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_gets_a_notice_email(ApiFixture fixture)
{
    private const string NoticeSubject = "Se agregó una passkey a tu cuenta";

    [Fact]
    public async Task Then_the_account_gets_a_notice_email_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, command);
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();

        await owner.AddPasskeyAsync(authenticator);

        var notice = await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(command.Email!, email => email.Subject == NoticeSubject);
        notice.TextBody.ShouldContain(PasskeyRequests.PasskeyName);
    }
}
