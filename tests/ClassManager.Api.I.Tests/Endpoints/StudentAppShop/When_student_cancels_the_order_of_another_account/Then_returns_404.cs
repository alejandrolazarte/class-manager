namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_cancels_the_order_of_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherStudent = await fixture.InviteStudentAppOfAsync(scenario.Coaches, CoachScenario.OtherStudentFullName);
        var classPack = await scenario.Coaches.Business.HttpClient.CreateClassPackAsync();
        var otherOrder = await otherStudent.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        using var response = await scenario.Student.PutStudentAppOrderCancellationAsync(otherOrder.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await scenario.Student.ListStudentAppOrdersAsync()).ShouldBeEmpty();
    }
}
