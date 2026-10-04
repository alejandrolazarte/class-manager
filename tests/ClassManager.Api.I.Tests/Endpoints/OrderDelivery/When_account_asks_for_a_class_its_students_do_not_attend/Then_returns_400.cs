using ClassManager.Core.Domain.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_account_asks_for_a_class_its_students_do_not_attend;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);

        using var response = await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Coaches.OtherClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(OrderErrorCodes.ClassNotValid);
    }
}
