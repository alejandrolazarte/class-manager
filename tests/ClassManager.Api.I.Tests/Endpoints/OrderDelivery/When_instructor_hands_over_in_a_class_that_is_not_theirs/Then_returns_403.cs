namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_instructor_hands_over_in_a_class_that_is_not_theirs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherStudent = await fixture.InviteStudentAppOfAsync(scenario.Instructors, InstructorScenario.OtherStudentFullName);
        var owner = scenario.Instructors.Business.HttpClient;
        var otherClassGroupId = scenario.Instructors.OtherClassGroup.Id;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        using (var placed = await otherStudent.Student.PostStudentAppOrderForClassAsync(
            otherClassGroupId, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1)))
        {
            placed.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var orderId = (await otherStudent.Student.ListStudentAppOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();

        using var response = await scenario.Instructors.Instructor.PutClassDeliveryAsync(otherClassGroupId, orderId);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
