namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_team_member_calls_a_student_endpoint;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.GetAsync(new Uri(ApiRoutes.StudentApp, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
