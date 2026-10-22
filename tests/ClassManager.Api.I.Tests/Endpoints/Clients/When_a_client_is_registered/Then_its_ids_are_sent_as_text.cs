using System.Text.Json;

using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_a_client_is_registered;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_ids_are_sent_as_text(ApiFixture fixture)
{
    private const string IdProperty = "id";
    private const string ClientIdProperty = "clientId";
    private const string StudentsProperty = "students";

    [Fact]
    public async Task Then_its_ids_are_sent_as_text_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var client = body.RootElement;
        var student = client.GetProperty(StudentsProperty)[0];
        (client.GetProperty(IdProperty).ValueKind, student.GetProperty(IdProperty).ValueKind, student.GetProperty(ClientIdProperty).ValueKind)
            .ShouldBe((JsonValueKind.String, JsonValueKind.String, JsonValueKind.String));
    }
}
