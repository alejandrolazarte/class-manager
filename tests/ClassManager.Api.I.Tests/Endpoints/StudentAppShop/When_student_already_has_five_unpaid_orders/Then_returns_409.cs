using ClassManager.Core.Domain.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_already_has_five_unpaid_orders;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classPack = await scenario.Coaches.Business.HttpClient.CreateClassPackAsync();
        for (var orderNumber = 0; orderNumber < Order.MaximumOpenRequestsPerClient; orderNumber++)
        {
            await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));
        }

        using var response = await scenario.Student.PostStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(OrderErrorCodes.TooManyOpen);
    }
}
