namespace ClassManager.Api.I.Tests.Endpoints.Students.When_adding_student_with_taken_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        await business.HttpClient.AddStudentAsync(client.Id, "Tomás Pérez");

        using var response = await business.HttpClient.PostStudentAsync(client.Id, "tomás pérez");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
