namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_student_places_an_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_owner_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_owner_is_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);

        (await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Instructors.InstructorClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();

        var notification = (await owner.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldStartWith("Nuevo pedido n.º 1 de");
        notification.Url.ShouldBe("/today/orders");
    }
}
