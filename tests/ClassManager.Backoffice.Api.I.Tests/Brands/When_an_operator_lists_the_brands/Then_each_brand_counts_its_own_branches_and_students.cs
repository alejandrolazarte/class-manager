using System.Net.Http.Json;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_lists_the_brands;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_brand_counts_its_own_branches_and_students(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_each_brand_counts_its_own_branches_and_students_Run()
    {
        var studio = await fixture.SeedBrandAsync("Estudio", [2, 1], brandId => [Subscription.Start(brandId, "pro", 29m, "EUR", null, BackofficeApiFactory.Now)]);
        var instructor = await fixture.SeedBrandAsync("Instructora", [4], brandId => [Subscription.Start(brandId, "lite", 12m, "EUR", null, BackofficeApiFactory.Now)]);
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync());

        using var response = await client.GetAsync(BackofficeRequests.BrandsRoute, accessToken);

        var brands = (await response.Content.ReadFromJsonAsync<BrandListResponse>())!.Brands;
        brands.Single(brand => brand.Id == studio.Id).ShouldBe(new BrandSummaryResponse(studio.Id, "Estudio", studio.OwnerEmail, "pro", "Active", 2, 3));
        brands.Single(brand => brand.Id == instructor.Id).ShouldBe(new BrandSummaryResponse(instructor.Id, "Instructora", instructor.OwnerEmail, "lite", "Active", 1, 4));
    }
}
