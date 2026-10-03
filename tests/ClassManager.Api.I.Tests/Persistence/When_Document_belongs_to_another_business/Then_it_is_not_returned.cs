using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_Document_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherProduct = await otherBusiness.HttpClient.CreateProductAsync();
        var otherImage = (await otherBusiness.HttpClient.AddProductImageAsync(otherProduct.Id)).Images.Single();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.Documents.AnyAsync(document => document.Id == otherImage.Id)).ShouldBeFalse();
    }
}
