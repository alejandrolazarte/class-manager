using ClassManager.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Email.When_an_email_is_sent;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_font_is_served_by_the_web_app(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_font_is_served_by_the_web_app_Run()
    {
        var ownerCommand = AuthenticationRequests.SignUpCommand();
        using var anonymous = fixture.ApiFactory.CreateClient();
        await anonymous.SignUpAsync(ownerCommand);
        var webAppUrl = fixture.ApiFactory.Services.GetRequiredService<IConfiguration>()[WebAppLinks.WebAppUrlSetting]!.TrimEnd('/');

        await anonymous.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailTransport, ownerCommand.Email!);

        var sentEmail = fixture.ApiFactory.EmailTransport.SentTo(ownerCommand.Email!)[^1];
        sentEmail.HtmlBody.ShouldContain("url('" + webAppUrl + WebAppLinks.WebFontsPath);
    }
}
