using ClassManager.Core.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_adds_a_product_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_public_document_is_recorded(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_public_document_is_recorded_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();

        var image = (await business.HttpClient.AddProductImageAsync(product.Id)).Images.Single();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var document = await context.Documents.SingleAsync(candidate => candidate.Id == image.Id);
        document.Visibility.ShouldBe(DocumentVisibility.Public);
        document.SizeInBytes.ShouldBe(CatalogImageRequests.PngImage.Length);
        document.Path.ShouldStartWith($"{business.Business.Id}/products/{product.Id}/");
    }
}
