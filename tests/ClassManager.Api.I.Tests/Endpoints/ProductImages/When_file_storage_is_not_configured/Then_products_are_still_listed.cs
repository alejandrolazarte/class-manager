using ClassManager.Storage.AzureBlob.Blobs;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_file_storage_is_not_configured;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_products_are_still_listed(ApiFixture fixture)
{
    private const string ConnectionStringsSection = "ConnectionStrings";

    [Fact]
    public async Task Then_products_are_still_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateProductAsync();
        var settings = new Dictionary<string, string> { [$"{ConnectionStringsSection}:{BlobStorageOptions.ConnectionStringName}"] = string.Empty };
        await using var apiFactory = new BusinessApiFactory(fixture.ConnectionString, new FakeTimeProvider(BusinessApiFactory.Now), settings);
        using var client = apiFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = business.HttpClient.DefaultRequestHeaders.Authorization;

        var products = await client.ListProductsAsync();

        products.Count.ShouldBe(1);
    }
}
