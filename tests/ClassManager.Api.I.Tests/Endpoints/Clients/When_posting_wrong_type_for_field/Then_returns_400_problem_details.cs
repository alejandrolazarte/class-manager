using System.Text;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_wrong_type_for_field;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_problem_details(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_problem_details_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var body = Encoding.UTF8.GetBytes("{\"fullName\": 123, \"phoneNumber\": \"11 2233-4455\"}");

        using var response = await business.HttpClient.PostRawClientBodyAsync(body);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(ApiRequests.ProblemJsonMediaType);
        (await response.Content.ReadAsStringAsync()).ShouldNotContain(ApiRequests.InternalNamespacePrefix);
    }
}
