namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_opens_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_units_left_are_the_most_that_can_be_ordered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_units_left_are_the_most_that_can_be_ordered_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await owner.RestockAsync(await owner.CreateProductAsync(), 3);

        var shop = await scenario.Student.GetStudentAppShopAsync();

        shop.Products.Single().Variants[0].MaximumUnits.ShouldBe(3);
    }
}
