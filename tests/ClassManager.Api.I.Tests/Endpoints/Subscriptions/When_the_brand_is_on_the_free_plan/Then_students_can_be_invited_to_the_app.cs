using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_is_on_the_free_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_students_can_be_invited_to_the_app(ApiFixture fixture)
{
    [Fact]
    public async Task Then_students_can_be_invited_to_the_app_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var client = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostStudentAppInvitationAsync(client.Id, "alumno@example.com");

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
