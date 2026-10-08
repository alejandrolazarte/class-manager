namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_business_A_gets_a_notification;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_business_B_does_not_see_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_business_B_does_not_see_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);

        (await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Instructors.InstructorClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();

        (await owner.GetTeamNotificationsAsync())!.Items.ShouldNotBeEmpty();
        (await otherBusiness.HttpClient.GetTeamNotificationsAsync())!.Items.ShouldBeEmpty();
    }
}
