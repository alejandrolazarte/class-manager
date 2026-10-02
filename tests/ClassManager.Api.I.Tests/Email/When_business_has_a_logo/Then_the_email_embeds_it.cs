using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Email.When_business_has_a_logo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_embeds_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_embeds_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        (await business.HttpClient.PutBrandLogoAsync(BrandRequests.PngLogo)).EnsureSuccessStatusCode();
        var email = MemberRequests.UniqueInviteeEmail();

        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        var sentEmail = fixture.ApiFactory.EmailTransport.SentTo(email)[^1];
        sentEmail.InlineImages.ShouldHaveSingleItem().Content.ShouldBe(BrandRequests.PngLogo);
        sentEmail.HtmlBody.ShouldContain($"cid:{EmailBrand.LogoContentId}");
    }
}
