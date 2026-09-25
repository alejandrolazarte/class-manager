namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_client_without_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_with_field_error(ApiFixture fixture)
{
    private const string FullNameField = "fullName";
    private const string ErrorsProperty = "errors";

    [Fact]
    public async Task Then_returns_400_with_field_error_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostClientAsync(fullName: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ErrorsProperty).TryGetProperty(FullNameField, out _).ShouldBeTrue();
    }
}
