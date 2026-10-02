using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Brands;

namespace ClassManager.Api.I.Tests.Email.When_business_has_a_brand_color;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_header_uses_it(ApiFixture fixture)
{
    private const string ThemeColor = "#7c3aed";

    [Fact]
    public async Task Then_the_email_header_uses_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        (await business.HttpClient.PutBrandAsync(new UpdateBrandCommand(null, ThemeColor, null, LocksTheme: false))).EnsureSuccessStatusCode();
        var email = MemberRequests.UniqueInviteeEmail();

        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        var sentEmail = fixture.ApiFactory.EmailTransport.SentTo(email)[^1];
        sentEmail.HtmlBody.ShouldContain($"background:{EmailPalette.From(ThemeColor, null).Primary};padding:28px 32px");
    }
}
