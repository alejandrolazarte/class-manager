namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_orders_more_than_in_stock;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 1);

        using var response = await scenario.Student.PostStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 2));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
