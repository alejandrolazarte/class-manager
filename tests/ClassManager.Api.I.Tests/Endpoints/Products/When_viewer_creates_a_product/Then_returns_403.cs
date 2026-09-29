using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Products.When_viewer_creates_a_product;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);

        using var response = await viewer.PostProductAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
