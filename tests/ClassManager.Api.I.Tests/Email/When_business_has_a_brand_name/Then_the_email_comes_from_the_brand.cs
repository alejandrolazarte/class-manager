using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Email.When_business_has_a_brand_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_comes_from_the_brand(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_comes_from_the_brand_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var brand = BrandRequests.DeltaBrand();
        (await business.HttpClient.PutBrandAsync(brand)).EnsureSuccessStatusCode();
        var email = MemberRequests.UniqueInviteeEmail();

        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        fixture.ApiFactory.EmailTransport.SentTo(email)[^1].FromName.ShouldBe(brand.BrandName);
    }
}
