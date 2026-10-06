using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_creating_instructor_with_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_email_is_returned(ApiFixture fixture)
{
    private const string FullName = "Lucía Díaz";
    private const string Email = "lucia@example.com";

    [Fact]
    public async Task Then_email_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Instructors, new CreateInstructorCommand(FullName, Email), ApiRequests.JsonOptions);

        var instructor = await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiRequests.JsonOptions);
        instructor!.Email.ShouldBe(Email);
    }
}
