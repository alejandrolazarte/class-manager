namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_the_brand_does_not_exist;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_answers_not_found(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_answers_not_found_Run()
    {
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync());

        using var response = await client.GetAsync($"{BackofficeRequests.BrandsRoute}/{Guid.CreateVersion7()}", accessToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
