using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_removes_a_product_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_document_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_document_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        var image = (await business.HttpClient.AddProductImageAsync(product.Id)).Images.Single();

        (await business.HttpClient.DeleteProductImageAsync(product.Id, image.Id)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.Documents.AnyAsync(document => document.Id == image.Id)).ShouldBeFalse();
    }
}
