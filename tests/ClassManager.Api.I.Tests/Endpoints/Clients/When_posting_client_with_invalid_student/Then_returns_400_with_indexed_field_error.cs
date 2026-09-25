using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_client_with_invalid_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_with_indexed_field_error(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_with_indexed_field_error_Run()
    {
        const string SecondStudentFullNameField = "students[1].FullName";
        const string ErrorsProperty = "errors";
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostClientAsync(
            students: [new NewStudent("Tomás Pérez", null, null), new NewStudent("L", null, null)]);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty(ErrorsProperty).TryGetProperty(SecondStudentFullNameField, out _).ShouldBeTrue();
    }
}
